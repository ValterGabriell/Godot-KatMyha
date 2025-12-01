using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KatMyha.Scripts.Dialog
{
    public class DtoBaseDialog
    {
        private string DialogText = null!;
         private Texture2D CurrentSprite = null!;
         private DtoBaseDialog Next = null!;
        
        public DtoBaseDialog(string dialogText, Texture2D currentSprite)
        {
            DialogText = dialogText;
            CurrentSprite = currentSprite;
        }

        public void SetNext(DtoBaseDialog next)
        {
            Next = next;
        }

        public string GetDialogText()
        {
            return DialogText;
        }

        public Texture2D GetCurrentSprite()
        {
            return CurrentSprite;
        }



    }
}
