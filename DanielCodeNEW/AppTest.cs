using System;
using System.Windows.Forms;

class Program:Form{
	public Button b1;
	public Program():base(){
		b1= new Button();
		b1.Text="Click Me";
		b1.Click+=new System.EventHandler(b1_click);
		this.Controls.Add(b1);
	}
	public void b1_click(Object Sender, EventArgs e){
		MessageBox.Show("ayy lmao");
	}

}
public static class main{
	public static void Main(string[] args){
		Application.Run(new Program());
	}
}