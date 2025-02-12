using System.Collections;

namespace OpenDental.UI
{

    ///<summary>A strongly typed collection of ODToolBarButtons</summary>
    public class ODToolBarButtonCollection : CollectionBase
    {

        ///<summary>Returns the Button with the given index.</summary>
        public ODToolBarButton this[int index]
        {
            get
            {
                return ((ODToolBarButton)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }

        ///<summary>Returns the Button with the given string tag.</summary>
        public ODToolBarButton this[string buttonTag]
        {
            get
            {
                for (int i = 0; i < List.Count; i++)
                {
                    if (((ODToolBarButton)List[i]).Tag.ToString() == buttonTag)
                    {
                        return ((ODToolBarButton)List[i]);
                    }
                }
                return null;
            }
        }

        ///<summary></summary>
        public void Add(ODToolBarButton button)
        {
            List.Add(button);
        }

        ///<summary></summary>
        public void Remove(int index)
        {
            if ((index > Count - 1) || (index < 0))
            {
                throw new System.IndexOutOfRangeException();
            }
            else
            {
                List.RemoveAt(index);
            }
        }

        /*
		///<summary>The button is retrieved from List and explicitly cast to the button type.</summary>
		public ODToolBarButton Item(int index){
			return (ODToolBarButton)List[index];
		}*/

        ///<summary></summary>
        public int IndexOf(ODToolBarButton value)
        {
            return (List.IndexOf(value));
        }

        ///<summary>Returns the index of the button for the given tag. Returns -1 if a no button is found that matches the tag.</summary>
        public int IndexOf(object buttonTag)
        {
            for (int i = 0; i < List.Count; i++)
            {
                if (((ODToolBarButton)List[i]).Tag == buttonTag)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}




