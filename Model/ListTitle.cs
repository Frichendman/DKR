using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace KR.Model
{
    public class ListTitle : ObservableCollection<Title>
    {
        public ListTitle()

        {

            var titles = PageEmployee.DataEntitiesEmployee.Title;

            var queryTitle = from title in titles select title;

            foreach (Title titl in queryTitle)

            {

                this.Add(titl);

            }

        }

    }
}
