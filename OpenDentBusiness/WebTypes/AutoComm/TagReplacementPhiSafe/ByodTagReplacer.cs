﻿using System.Text;

namespace OpenDentBusiness.AutoComm
{
    public class ByodTagReplacer : ApptTagReplacer
    {
        public const string BYOD_TAG = "[eClipboardBYOD]";

        ///<summary>Replaces appointment related tags.</summary>
        protected override void ReplaceTagsChild(StringBuilder sbTemplate, AutoCommObj autoCommObj, bool isEmail)
        {
            base.ReplaceTagsChild(sbTemplate, autoCommObj, isEmail);
            if (autoCommObj is Byod byod)
            {
                ReplaceOneTag(sbTemplate, BYOD_TAG, byod.ShortUrl, isEmail);
            }
        }
    }
}
