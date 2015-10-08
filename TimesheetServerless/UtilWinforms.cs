using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;


//Utility class with some helpers
namespace TimesheetServerless
{
    public static class UtilWinforms
    {	
		
        //Get controls of type
        public static IEnumerable<Control> GetAllControlsOfType(Control control, Type type)
        {
            IEnumerable<Control> controls = control.Controls.Cast<Control>();
            return controls.SelectMany(c => GetAllControlsOfType(c, type))
                .Concat(controls)
                .Where(c => c.GetType() == type);                                   //Item of same type
        }

        //Get controls BUT items of type
        public static IEnumerable<Control> GetAllControlsBut(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();
            return controls.SelectMany(c => GetAllControlsOfType(c, type))
                .Concat(controls)
                .Where(c => c.GetType() != type);                                   //Items but item of type
        }


        //Get ALL controls
        public static IEnumerable<Control> GetAllControls(Control control)
        {
            var controls = control.Controls.Cast<Control>();
            return controls.SelectMany(c => GetAllControls(c))
                .Concat(controls);                                   //Items but item of type
        }

        
        //Collections Utils
        public static void ClearText(ref List<Control> a)
        {
            foreach (var e in a)
            {
                e.Text = "";
            }
        }


        /*
         * Searches table for changes, and updates autocomplete based on the name of textboxes' substrings
         * given they have the properties names - reflection 
         * -----------------------------------------------------------------------------------------------------------------------
         * INPUT: 
         *          1. textBoxes are all text box controls to consider for autocompletion
         *          2. databaseUnits holds the list of employees or workweeks
         * NEED: 
         *          1. Text boxes prefixed with "txt", with stem equals to property; format: [txt][name of property]
         *          2. Following the prefix "txt" is the property name (eg "txtFirstName")
         *          3. Generic reference properties MUST be named the same as the TextBox stems.
         * Called from Form1, and Form2
         *          
         * Populate allEmployees and text boxes with AutoComplete by reflection,
         * in case there is an addition of employee since table loaded
         */
        public static void UpdateAutoComplete<T>(ref List<Control> textBoxes, ref List<T> databaseUnits, int prefixSize = 3)
        {
            if(databaseUnits != null)
			{
				foreach (TextBox e in textBoxes)
				{
					e.AutoCompleteMode = AutoCompleteMode.Suggest;
					e.AutoCompleteSource = AutoCompleteSource.CustomSource;

					AutoCompleteStringCollection data = new AutoCompleteStringCollection();

					//strb has name of property to call
					StringBuilder strb = new StringBuilder(e.Name, prefixSize, e.Name.Length - prefixSize, 20);       //string substr, length, ini reserved num char
					List<string> tempAutoComplete = new List<string>();

					//Reflection; get field inside employee of name "strb"
					Type type = typeof(T);
					PropertyInfo property = type.GetProperty(strb.ToString());              //Calling property

					try
					{
						foreach (T item in databaseUnits)
						{
							tempAutoComplete.Add(property.GetValue(item, null).ToString().Trim());
						}
					}
					catch (Exception exc)
					{

						MessageBox.Show(String.Format("Error: {0}", exc.ToString()));
					}
                
					data.AddRange(tempAutoComplete.ToArray());
					e.AutoCompleteCustomSource = data;                
				}
			}
        }


        /*
         * Generic for all autocompletes self update
         * Note: as in type something, the rest of the allTextBoxControls get show related info
         */
        public static void AutoCompleteEmpties<T>(ref List<Control> allTextBoxControls, T unit, int prefixSize = 3)
        {
            //Search for all text boxes     
            for (int i = 0; i < allTextBoxControls.Count(); i++)
            {                
                //Reflection; get field inside employee of name stem
                Type type = typeof(T);
                PropertyInfo property = type.GetProperty(allTextBoxControls[i].Name.Substring(prefixSize));   //Calling property of name

                if (property != null && property.GetValue(unit, null).ToString().Trim() != String.Empty)
                {
                    allTextBoxControls[i].Text = property.GetValue(unit, null).ToString().Trim();     //Get value of property of unit class
                }                
            }                      
        }	
    }
}
