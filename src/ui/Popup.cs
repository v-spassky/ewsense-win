using ewsense.winapi;

namespace ewsense.ui;

public static class Popup
{
    public static void ShowKeywordDetectedPopup()
    {
        var caretPos = WindowsApi.GetCaretPosition();

        var popup = new Form
        {
            Size = new Size(200, 100),
            FormBorderStyle = FormBorderStyle.None,
            BackColor = Color.LightYellow,
            StartPosition = FormStartPosition.Manual,
            Location = caretPos with { Y = caretPos.Y + 5 },
            TopMost = true
        };

        var message = new Label
        {
            Text = "Keyword detected!",
            AutoSize = true,
            Font = new Font("Arial", 12, FontStyle.Bold),
            Location = new Point(50, 40)
        };

        popup.Controls.Add(message);
        popup.Show();

        var timer = new System.Windows.Forms.Timer();
        timer.Interval = 2000;
        timer.Tick += (_, _) =>
        {
            timer.Stop();
            timer.Dispose();
            popup.Close();
            popup.Dispose();
        };
        timer.Start();
    }
}
