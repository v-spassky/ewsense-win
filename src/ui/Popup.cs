using ewsense.winapi;

namespace ewsense.ui;

public class Popup
{
    public void ShowKeywordDetectedPopup()
    {
        Point caretPos = WindowsApi.GetCaretPosition();

        Form popup = new Form
        {
            Size = new Size(200, 100),
            FormBorderStyle = FormBorderStyle.None,
            BackColor = Color.LightYellow,
            StartPosition = FormStartPosition.Manual,
            Location = new Point(caretPos.X, caretPos.Y + 5),
            TopMost = true
        };

        Label message = new Label
        {
            Text = "Keyword detected!",
            AutoSize = true,
            Font = new Font("Arial", 12, FontStyle.Bold),
            Location = new Point(50, 40)
        };

        popup.Controls.Add(message);
        popup.Show();

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        timer.Interval = 2000;
        timer.Tick += (sender, e) =>
        {
            timer.Stop();
            timer.Dispose();
            popup.Close();
            popup.Dispose();
        };
        timer.Start();
    }
}
