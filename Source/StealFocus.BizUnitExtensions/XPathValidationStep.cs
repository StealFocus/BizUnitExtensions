namespace StealFocus.BizUnitExtensions
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Xml;
    using System.Xml.XPath;

    using BizUnit;
    using BizUnit.Common;
    using BizUnit.TestSteps.Common;
    using BizUnit.Xaml;

    [CLSCompliant(false)]
    public class XPathValidationStep : SubStepBase
    {
        public XPathValidationStep()
        {
            this.XPathValidations = new Collection<XPathDefinition>();
        }

        public Collection<XPathDefinition> XPathValidations { get; private set; }

        public override Stream Execute(Stream data, Context context)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            XmlReader reader = XmlReader.Create(data);
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(reader);
            this.ValidateXPathExpressions(xmlDocument, context);
            data.Seek(0L, SeekOrigin.Begin);
            return data;
        }

        public override void Validate(Context context)
        {
            foreach (XPathDefinition xpathDefinition in this.XPathValidations)
            {
                ArgumentValidation.CheckForNullReference(xpathDefinition.XPath, "xpath.XPath");
                ArgumentValidation.CheckForNullReference(xpathDefinition.Value, "xpath.Value");
            }
        }

        private void ValidateXPathExpressions(XmlDocument doc, Context context)
        {
            foreach (XPathDefinition xpathDefinition in this.XPathValidations)
            {
                string xpath = xpathDefinition.XPath;
                string str = xpathDefinition.Value;
                if (null != xpathDefinition.Description)
                {
                    context.LogInfo("XPath: {0}", new object[] { xpathDefinition.Description });
                }

                context.LogInfo("Evaluating XPath {0} equals \"{1}\"", (object)xpath, (object)str);
                object obj = doc.CreateNavigator().Evaluate(xpath);
                string strB;
                if (obj.GetType().Name == "XPathSelectionIterator")
                {
                    XPathNodeIterator xpathNodeIterator = (XPathNodeIterator)obj;
                    xpathNodeIterator.MoveNext();
                    strB = xpathNodeIterator.Current.ToString();
                }
                else
                {
                    strB = obj.ToString();
                }

                if (!string.IsNullOrEmpty(xpathDefinition.ContextKey))
                {
                    context.Add(xpathDefinition.ContextKey, strB);
                }

                if (!string.IsNullOrEmpty(str))
                {
                    if (0 != string.Compare(str, strB, StringComparison.CurrentCulture))
                    {
                        context.LogError("XPath evaluation failed. Expected:<{0}>. Actual:<{1}>.", (object)str, (object)strB);
                        throw new BizUnitExtensionsException(string.Format(CultureInfo.CurrentCulture, "XmlValidationStep failed, compare {0} != {1}, xpath query used: {2}", str, strB, xpath));
                    }
                    
                    context.LogInfo("XPath evaluation succeeded. Expected:<{0}>. Actual:<{1}>.", (object)str, (object)strB);
                }
            }
        }
    }
}
