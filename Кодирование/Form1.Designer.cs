namespace Кодирование;

partial class ConverterForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        groupBox1 = new GroupBox();
        radioDouble = new RadioButton();
        radioFloat = new RadioButton();
        label7 = new Label();
        textBox3 = new TextBox();
        label6 = new Label();
        additionalCodeEnabled = new CheckBox();
        numericUpDown1 = new NumericUpDown();
        textBox2 = new TextBox();
        textBox1 = new TextBox();
        label5 = new Label();
        label4 = new Label();
        number_system_to = new NumericUpDown();
        number_system_from = new NumericUpDown();
        number_system_switch_button = new Button();
        label3 = new Label();
        label2 = new Label();
        label1 = new Label();
        backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
        groupBox1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
        ((System.ComponentModel.ISupportInitialize)number_system_to).BeginInit();
        ((System.ComponentModel.ISupportInitialize)number_system_from).BeginInit();
        SuspendLayout();
        // 
        // groupBox1
        // 
        groupBox1.Controls.Add(radioDouble);
        groupBox1.Controls.Add(radioFloat);
        groupBox1.Controls.Add(label7);
        groupBox1.Controls.Add(textBox3);
        groupBox1.Controls.Add(label6);
        groupBox1.Controls.Add(additionalCodeEnabled);
        groupBox1.Controls.Add(numericUpDown1);
        groupBox1.Controls.Add(textBox2);
        groupBox1.Controls.Add(textBox1);
        groupBox1.Controls.Add(label5);
        groupBox1.Controls.Add(label4);
        groupBox1.Controls.Add(number_system_to);
        groupBox1.Controls.Add(number_system_from);
        groupBox1.Controls.Add(number_system_switch_button);
        groupBox1.Controls.Add(label3);
        groupBox1.Controls.Add(label2);
        groupBox1.Controls.Add(label1);
        groupBox1.Location = new Point(23, 22);
        groupBox1.Name = "groupBox1";
        groupBox1.Size = new Size(445, 386);
        groupBox1.TabIndex = 0;
        groupBox1.TabStop = false;
        groupBox1.Text = "Перевод между системами счисления";
        // 
        // radioDouble
        // 
        radioDouble.AutoSize = true;
        radioDouble.Location = new Point(79, 260);
        radioDouble.Name = "radioDouble";
        radioDouble.Size = new Size(62, 19);
        radioDouble.TabIndex = 22;
        radioDouble.TabStop = true;
        radioDouble.Text = "double";
        radioDouble.UseVisualStyleBackColor = true;
        radioDouble.CheckedChanged += radioDouble_CheckedChanged;
        // 
        // radioFloat
        // 
        radioFloat.AutoSize = true;
        radioFloat.Checked = true;
        radioFloat.Location = new Point(24, 260);
        radioFloat.Name = "radioFloat";
        radioFloat.Size = new Size(49, 19);
        radioFloat.TabIndex = 21;
        radioFloat.TabStop = true;
        radioFloat.Text = "float";
        radioFloat.UseVisualStyleBackColor = true;
        radioFloat.CheckedChanged += radioFloat_CheckedChanged;
        // 
        // label7
        // 
        label7.AutoSize = true;
        label7.Location = new Point(24, 242);
        label7.Name = "label7";
        label7.Size = new Size(323, 15);
        label7.TabIndex = 19;
        label7.Text = "Как выглядят вещественные числа в памяти компьютера";
        // 
        // textBox3
        // 
        textBox3.Location = new Point(24, 285);
        textBox3.Name = "textBox3";
        textBox3.ReadOnly = true;
        textBox3.Size = new Size(408, 23);
        textBox3.TabIndex = 18;
        textBox3.Text = "0";
        // 
        // label6
        // 
        label6.AutoSize = true;
        label6.Location = new Point(76, 148);
        label6.Name = "label6";
        label6.Size = new Size(94, 15);
        label6.TabIndex = 17;
        label6.Text = "Количество бит";
        // 
        // additionalCodeEnabled
        // 
        additionalCodeEnabled.AutoSize = true;
        additionalCodeEnabled.Checked = true;
        additionalCodeEnabled.CheckState = CheckState.Checked;
        additionalCodeEnabled.Location = new Point(176, 176);
        additionalCodeEnabled.Name = "additionalCodeEnabled";
        additionalCodeEnabled.Size = new Size(134, 19);
        additionalCodeEnabled.TabIndex = 16;
        additionalCodeEnabled.Text = "вычислить доп. код";
        additionalCodeEnabled.UseVisualStyleBackColor = true;
        additionalCodeEnabled.CheckedChanged += additionalCodeEnabled_CheckedChanged;
        // 
        // numericUpDown1
        // 
        numericUpDown1.Location = new Point(176, 146);
        numericUpDown1.Name = "numericUpDown1";
        numericUpDown1.Size = new Size(120, 23);
        numericUpDown1.TabIndex = 14;
        numericUpDown1.Value = new decimal(new int[] { 8, 0, 0, 0 });
        numericUpDown1.ValueChanged += number_ValueChanged;
        // 
        // textBox2
        // 
        textBox2.Location = new Point(24, 117);
        textBox2.Name = "textBox2";
        textBox2.Size = new Size(286, 23);
        textBox2.TabIndex = 13;
        textBox2.Text = "1";
        textBox2.TextChanged += originNumber_TextChanged;
        // 
        // textBox1
        // 
        textBox1.Location = new Point(24, 198);
        textBox1.Name = "textBox1";
        textBox1.ReadOnly = true;
        textBox1.Size = new Size(408, 23);
        textBox1.TabIndex = 12;
        textBox1.Text = "0";
        // 
        // label5
        // 
        label5.AutoSize = true;
        label5.Location = new Point(24, 180);
        label5.Name = "label5";
        label5.Size = new Size(142, 15);
        label5.TabIndex = 11;
        label5.Text = "результат (целые числа)";
        // 
        // label4
        // 
        label4.AutoSize = true;
        label4.Location = new Point(24, 99);
        label4.Name = "label4";
        label4.Size = new Size(96, 15);
        label4.TabIndex = 9;
        label4.Text = "исходное число";
        // 
        // number_system_to
        // 
        number_system_to.Location = new Point(187, 56);
        number_system_to.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
        number_system_to.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
        number_system_to.Name = "number_system_to";
        number_system_to.Size = new Size(65, 23);
        number_system_to.TabIndex = 7;
        number_system_to.Value = new decimal(new int[] { 2, 0, 0, 0 });
        number_system_to.ValueChanged += number_ValueChanged;
        // 
        // number_system_from
        // 
        number_system_from.Location = new Point(24, 56);
        number_system_from.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
        number_system_from.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
        number_system_from.Name = "number_system_from";
        number_system_from.Size = new Size(65, 23);
        number_system_from.TabIndex = 6;
        number_system_from.Value = new decimal(new int[] { 10, 0, 0, 0 });
        number_system_from.ValueChanged += number_ValueChanged;
        // 
        // number_system_switch_button
        // 
        number_system_switch_button.Location = new Point(118, 56);
        number_system_switch_button.Name = "number_system_switch_button";
        number_system_switch_button.Size = new Size(51, 23);
        number_system_switch_button.TabIndex = 5;
        number_system_switch_button.Text = "<-|->";
        number_system_switch_button.UseVisualStyleBackColor = true;
        number_system_switch_button.Click += number_system_switch_button_Click;
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Location = new Point(187, 38);
        label3.Name = "label3";
        label3.Size = new Size(19, 15);
        label3.TabIndex = 4;
        label3.Text = "To";
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(24, 38);
        label2.Name = "label2";
        label2.Size = new Size(35, 15);
        label2.TabIndex = 3;
        label2.Text = "From";
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(22, 19);
        label1.Name = "label1";
        label1.Size = new Size(135, 15);
        label1.TabIndex = 2;
        label1.Text = "Направление перевода";
        // 
        // ConverterForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 450);
        Controls.Add(groupBox1);
        Name = "ConverterForm";
        Text = "Числа";
        groupBox1.ResumeLayout(false);
        groupBox1.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
        ((System.ComponentModel.ISupportInitialize)number_system_to).EndInit();
        ((System.ComponentModel.ISupportInitialize)number_system_from).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private GroupBox groupBox1;
    private Label label1;
    private Label label2;
    private Button number_system_switch_button;
    private Label label3;
    private System.ComponentModel.BackgroundWorker backgroundWorker1;
    private NumericUpDown number_system_to;
    private NumericUpDown number_system_from;
    private Label label5;
    private Label label4;
    private TextBox textBox1;
    private TextBox textBox2;
    private NumericUpDown numericUpDown1;
    private CheckBox additionalCodeEnabled;
    private Label label6;
    private RadioButton radioDouble;
    private RadioButton radioFloat;
    private Label label7;
    private TextBox textBox3;
}