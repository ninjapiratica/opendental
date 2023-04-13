using OpenDentBusiness;

namespace UnitTestsCore
{
    public class EmailHostingTemplateT
    {
        public static EmailHostingTemplate CreateEmailHostingTemplate(long clinicNum, PromotionType templateType)
        {
            EmailHostingTemplate template = EmailHostingTemplates.CreateDefaultTemplate(clinicNum, templateType);
            EmailHostingTemplates.Insert(template);
            return template;
        }
    }
}
