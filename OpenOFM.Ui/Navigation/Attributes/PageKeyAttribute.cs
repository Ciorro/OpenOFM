namespace OpenOFM.Ui.Navigation.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    sealed class PageKeyAttribute : Attribute
    {
        public object PageKey { get; }

        public PageKeyAttribute(object key)
        {
            PageKey = key;
        }
    }

}
