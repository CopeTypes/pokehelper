using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokeHelper.ui;

public partial class InputboxUI : Form
{

    private readonly Action _buttonEvent;
    private readonly string _uiTitle;
    private readonly string _buttonText;
    
    public InputboxUI()
    {
        InitializeComponent();
    }

    public InputboxUI(string title, string btnText, Action onClick)
    {
        InitializeComponent();
        _uiTitle = title;
        _buttonText = btnText;
        _buttonEvent = onClick;
    }

    
    private async void okButton_Click(object sender, EventArgs e)
    {
        await Task.Run(_buttonEvent);
    }

    private void InputboxUI_Shown(object sender, EventArgs e)
    {
        Text = _uiTitle;
        okButton.Text = _buttonText;
    }
}