using Converter = NumericSystemConverter.NumericSystemConverter;

// ReSharper disable SpecifyACultureInStringConversionExplicitly

namespace NumericSystemConverter;

public partial class ConverterForm : Form
{
    private static readonly int MaxLength = long.MaxValue.ToString().Length;

    public ConverterForm()
    {
        InitializeComponent();
        Recalculate();
    }

    private void number_ValueChanged(object sender, EventArgs e)
    {
        Recalculate();
    }

    private void number_system_switch_button_Click(object sender, EventArgs e)
    {
        (number_system_from.Value, number_system_to.Value) = (number_system_to.Value, number_system_from.Value);
        Recalculate();
    }

    private void originNumber_TextChanged(object sender, EventArgs e)
    {
        Recalculate();
    }

    private void additionalCodeEnabled_CheckedChanged(object sender, EventArgs e)
    {
        Recalculate();
    }

    private void radioFloat_CheckedChanged(object sender, EventArgs e)
    {
        Recalculate();
    }

    private void radioDouble_CheckedChanged(object sender, EventArgs e)
    {
        Recalculate();
    }

    private void Recalculate()
    {
        var originNumberStr = textBox2.Text.Trim().Replace('.', ',');
        var fromSystem = (int)number_system_from.Value;
        var toSystem = (int)number_system_to.Value;

        if (!IsCorrect(fromSystem, originNumberStr))
        {
            textBox1.Text = "Некорректный ввод";
            return;
        }

        var num = decimal.Parse(originNumberStr);
        if (num - (long)num == 0 && long.TryParse(originNumberStr, out var originalNum))
        {
            textBox3.Text = "-";
            var isNegative = originalNum < 0;

            var resultNum = NumericSystemConverter.Recalculate(fromSystem, toSystem, Math.Abs(originalNum));
            var result = AddBytes(resultNum);

            if (result[0] != '0')
            {
                textBox1.Text = "Что-то не так, не влазит значащий бит, укажите больше битов";
                return;
            }

            WriteToResult(toSystem, isNegative, result);
        }
        else
        {
            textBox1.Text = "-";
            if (fromSystem != 10 || toSystem != 2)
            {
                textBox3.Text = "Поддерживается только перевод из 10-ой в 2-ную систему";
                return;
            }

            if(radioFloat.Checked)
                textBox3.Text = NumericSystemConverter.CalculateFloatingNumber10To2(num, NumericSystemConverter.Floating.Float);
            else if(radioDouble.Checked)
                textBox3.Text = NumericSystemConverter.CalculateFloatingNumber10To2(num, NumericSystemConverter.Floating.Double);
        }
    }

    private void WriteToResult(int toSystem, bool isNegative, string result)
    {
        if (isNegative)
        {
            if (additionalCodeEnabled.Checked)
                textBox1.Text = NumericSystemConverter.CalculateAdditionalCode(toSystem, result);
            else
                textBox1.Text = '-' + result;
        }
        else
        {
            textBox1.Text = result;
        }
    }

    private static bool IsCorrect(int sysFrom, string number)
    {
        if (string.IsNullOrWhiteSpace(number)
            || !char.IsDigit(number.Last())
            || MaxLength < number.Length
            || !decimal.TryParse(number, out var result))
            return false;

        if (result == 0)
            return true;

        var nums = number
            .Where(x => x != ',')
            .Skip(result < 0 ? 1 : 0)
            .Select(ch => int.Parse(ch.ToString()))
            .ToList();

        return nums.All(num => num < sysFrom);
    }

    private string AddBytes(string number)
    {
        var targetLength = (int)numericUpDown1.Value;
        var needWriting = targetLength - number.Length;
        if (needWriting > 0)
            number = new string('0', needWriting) + number;

        return number;
    }
}