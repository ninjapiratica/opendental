using System.Collections.Generic;

namespace OpenDentBusiness
{
    public class WebServiceMainHQReal : OpenDentBusiness.WebServiceMainHQ.WebServiceMainHQ, IWebServiceMainHQ
    {
        public List<long> GetEServiceClinicsAllowed(List<long> listClinicNums, eServiceCode eService)
        {
            return WebServiceMainHQProxy.GetEServiceClinicsAllowed(listClinicNums, eService);
        }
    }
}
