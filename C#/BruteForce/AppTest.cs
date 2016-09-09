using System;
using System.Windows.Forms;
using System.Drawing;
using System.Windows;
class Program:Form{
	public Button b1;
    public TextBox passturd;
	public Program():base(){
		b1= new Button();
        passturd = new TextBox();
        passturd.UseSystemPasswordChar = true;
        passturd.Location = new Point(50, 50);
		b1.Text="Click Me";
        this.Controls.Add(passturd);
		b1.Click+=new System.EventHandler(b1_click);
		this.Controls.Add(b1);
	}
	public void b1_click(Object Sender, EventArgs e){
        if (passturd.Text == "ehh1") {
            MessageBox.Show("Login Successful");
        }
        else
        {
            MessageBox.Show("Login Failed");
        }
	}

}
public static class main{
	public static void Main(string[] args){
		Application.Run(new Program());
	}
}