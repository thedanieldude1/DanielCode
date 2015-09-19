import javax.swing.*;
import java.awt.event.*;
class ButtonThing extends JPanel implements ActionListener{
	public ButtonThing(){
	JButton b1 = new JButton("Write");
	JTextField t1 = new JTextField(20);
	b1.setMnemonic(KeyEvent.VK_D);
	b1.setActionCommand("write");
	b1.setToolTipText("Write");
	b1.addActionListener(this);
	add(b1);
	add(t1);
	}
	public void actionPerformed(ActionEvent e){
		
	}
}
class Main{
	private static void createAndShowGUI() {
        	JFrame frame = new JFrame("ButtonDemo");
        	frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		frame.add(new ButtonThing());
		frame.setTitle("Arduino Interface");
        	frame.pack();
        	frame.setVisible(true);
		
    }
	public static void main(String []args){
		javax.swing.SwingUtilities.invokeLater(new Runnable() {
			public void run(){
				createAndShowGUI();
			}
		});
	}
}