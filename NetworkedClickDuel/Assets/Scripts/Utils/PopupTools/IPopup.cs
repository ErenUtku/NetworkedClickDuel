namespace Utils.PopupTools
{
    internal interface IPopup
    {
        void Show();
        void Hide();
        void InitFromDefinition(PopupDefinition definition);
    }
}