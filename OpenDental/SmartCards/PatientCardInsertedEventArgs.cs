using OpenDentBusiness;
using System;

namespace OpenDental.SmartCards
{
    public class PatientCardInsertedEventArgs : EventArgs
    {
        public PatientCardInsertedEventArgs(Patient patient)
        {
            this.patient = patient;
        }

        private Patient patient;
        public Patient Patient
        {
            get { return patient; }
        }
    }
}
