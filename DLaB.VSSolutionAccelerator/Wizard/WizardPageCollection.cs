using System.Collections.Generic;
using System.Linq;

namespace DLaB.VSSolutionAccelerator.Wizard
{
    public enum WizardPageLocation
    {
        Start,
        Middle,
        End
    }
    public class WizardPageCollection : Dictionary<int, IWizardPage>
    {
        #region Properties

        /// <summary>
        /// The current IWizardPage
        /// </summary>
        public IWizardPage CurrentPage { get; private set; }
        /// <summary>
        /// The first page in the collection
        /// </summary>
        public IWizardPage FirstPage
        {
            get { return this[this.Min(x => x.Key)]; }
        }
        /// <summary>
        /// The last page in the collection
        /// </summary>
        public IWizardPage LastPage
        {
            get { return this[this.Max(x => x.Key)]; }
        }

        /// <summary>
        /// The location of the current IWizardPage
        /// </summary>
        public WizardPageLocation PageLocation { get; private set; }

        /// <summary>
        /// <para>Determines whether the wizard is able to move to the next page.</para>
        /// <para>Will return false if Page Location is currently the last page.</para>
        /// <para>Otherwise, true.</para>
        /// </summary>
        public bool CanMoveNext
        {
            get
            {
                if (this.Count == 1)
                { return false; }

                if (this.Count > 0 &&
                    this.PageLocation != WizardPageLocation.End)
                {
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// <para>Determines whether the wizard is able to move to the previous page.</para>
        /// <para>Will return false if Page Location is currently the first page.</para>
        /// <para>Otherwise, true.</para>
        /// </summary>
        public bool CanMovePrevious
        {
            get
            {
                if (this.Count == 1)
                { return false; }

                if (this.Count > 0 &&
                    this.PageLocation != WizardPageLocation.Start)
                {
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region Constructor

        public WizardPageCollection()
        {
            PageLocation = WizardPageLocation.Start;
        }

        #endregion

        #region Delegates & Events

        public delegate void WizardPageLocationChangedEventHanlder(WizardPageLocationChangedEventArgs e);
        public event WizardPageLocationChangedEventHanlder WizardPageLocationChanged;

        #endregion

        #region Public Methods

        public void Add(IWizardPage page)
        {
            var max = Keys.Count == 0 ?
                -1 : Keys.Max();
            Add(max + 1, page);
        }

        /// <summary>
        /// Moves to the first page in the collection
        /// </summary>
        /// <returns>First page as IWizard</returns>
        public IWizardPage MovePageFirst()
        {
            int previousPageIndex = IndexOf(CurrentPage);

            PageLocation = WizardPageLocation.Start;
            // Find the index of the first page
            int firstPageIndex = (from x in this
                                  select x.Key).Min();

            // Set the current page to be the first page
            CurrentPage = this[firstPageIndex];

            NotifyPageChanged(previousPageIndex);

            return CurrentPage;
        }
        /// <summary>
        /// Moves to the last page in the collection
        /// </summary>
        /// <returns>Last page as IWizard</returns>
        public IWizardPage MovePageLast()
        {
            int previousPageIndex = IndexOf(CurrentPage);

            PageLocation = WizardPageLocation.End;
            // Find the index of the last page
            int lastPageIndex = (from x in this
                                 select x.Key).Max();

            // Set the current page to be the last page
            CurrentPage = this[lastPageIndex];

            NotifyPageChanged(previousPageIndex);

            return CurrentPage;
        }
        /// <summary>
        /// Moves to the next page in the collection
        /// </summary>
        /// <returns>Next page as IWizard</returns>
        public IWizardPage MovePageNext()
        {
            int previousPageIndex = IndexOf(CurrentPage);

            if (PageLocation != WizardPageLocation.End &&
                CurrentPage != null)
            {
                // Find the index of the next page
                int nextPageIndex = (from x in this
                                     where x.Key > IndexOf(CurrentPage)
                                     select x.Key).Min();

                // Find the index of the last page
                int lastPageIndex = (from x in this
                                     select x.Key).Max();

                // If the next page is the last page
                if (nextPageIndex == lastPageIndex)
                {
                    PageLocation = WizardPageLocation.End;
                }
                else { PageLocation = WizardPageLocation.Middle; }

                // Set the current page to be the next page                
                CurrentPage = this[nextPageIndex];
                NotifyPageChanged(previousPageIndex);

                return CurrentPage;
            }
            return null;
        }
        /// <summary>
        /// Moves to the previous page in the collection
        /// </summary>
        /// <returns>Previous page as IWizard</returns>
        public IWizardPage MovePagePrevious()
        {
            int prevPageIndex = IndexOf(CurrentPage);

            if (PageLocation != WizardPageLocation.Start &&
                CurrentPage != null)
            {
                // Find the index of the previous page
                int previousPageIndex = (from x in this
                                         where x.Key < IndexOf(CurrentPage)
                                         select x.Key).Max();

                // Find the index of the first page
                int firstPageIndex = (from x in this
                                      select x.Key).Min();

                // If the previous page is the first page
                if (previousPageIndex == firstPageIndex)
                {
                    PageLocation = WizardPageLocation.Start;
                }
                else { PageLocation = WizardPageLocation.Middle; }

                CurrentPage = this[previousPageIndex];

                NotifyPageChanged(prevPageIndex);

                return CurrentPage;
            }
            return null;
        }

        /// <summary>
        /// Find the page number of the current page
        /// </summary>
        /// <param name="wizardPage">The IWiwardPage whose page number to retrieve.</param>
        /// <returns>Page number for the given IWizardPage</returns>
        public int IndexOf(IWizardPage wizardPage)
        {
            foreach (KeyValuePair<int, IWizardPage> kv in this)
            {
                if (kv.Value.Equals(wizardPage))
                {
                    return kv.Key;
                }
            }
            return -1;
        }
        public void Reset()
        {
            CurrentPage = null;
            PageLocation = WizardPageLocation.Start;
        }

        #endregion

        #region private Methods

        private void NotifyPageChanged(int previousPageIndex)
        {
            if (WizardPageLocationChanged != null)
            {
                WizardPageLocationChangedEventArgs e = new WizardPageLocationChangedEventArgs();
                e.PageLocation = PageLocation;
                e.PageIndex = IndexOf(CurrentPage);
                e.PreviousPageIndex = previousPageIndex;
                WizardPageLocationChanged(e);
            }
        }

        #endregion
    }
    public class WizardPageLocationChangedEventArgs
    {
        /// <summary>
        /// The location of the current IWizardPage
        /// </summary>
        public WizardPageLocation PageLocation { get; set; }

        /// <summary>
        /// The page number of the current IWizardPage
        /// </summary>
        public int PageIndex { get; set; }

        public int PreviousPageIndex { get; set; }
    }
}