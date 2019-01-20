using System.Windows.Forms;

namespace DLaB.XrmToolBoxCommon
{
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption)
        {
            var prompt = new Form
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            var textLabel = new Label {Left = 50, Top = 20, Text = text, Width = 200};
            var textBox = new TextBox {Left = 50, Top = 50, Width = 400};
            var confirmation = new Button {Text = "Ok", Left = 350, Width = 100, Top = 70};
            confirmation.Click += (sender, e) => prompt.Close();
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;
            prompt.ShowDialog();
            return textBox.Text;
        }
    }
}
