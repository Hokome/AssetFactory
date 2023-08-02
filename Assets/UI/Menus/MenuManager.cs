using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetFactory.UI
{
    public class MenuManager : MonoBehaviour
    {
        public void OpenMenuSingle(Menu menu) => OpenMenuSingle(menu, MenuTransition.defaultTransition);
        public void OpenMenuSingle(Menu menu, MenuTransition transition)
        {

        }
        public void OpenMenuTracked(Menu menu) => OpenMenuTracked(menu, MenuTransition.defaultTransition);
        public void OpenMenuTracked(Menu menu, MenuTransition transition)
        {

        }
    }
}
