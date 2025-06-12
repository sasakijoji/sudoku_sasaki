using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;


namespace sudoku_sasaki
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string filePath = "";
        public MainWindow()
        {
            InitializeComponent();
            this.BtnClearData.IsEnabled = false;
            this.BtnComfirmData.IsEnabled = false;
        }
        /// <summary>
        /// 問題読み込みボタンが押下された時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLoadNumbers_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Environment.CurrentDirectory,
                Filter = "CSV Files (*.csv)|*.csv",
                FilterIndex = 1,
                RestoreDirectory = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                this.filePath = openFileDialog.FileName;
                // ファイルを開く処理
                List<List<int>> values = new List<List<int>>();
                if (ControlFile.ReadFile(this.filePath, ref values)
                    == true)
                {
                    displayValues(values);
                    this.BtnClearData.IsEnabled = true;
                    this.BtnComfirmData.IsEnabled = true;
                }
            }
        }
        /// <summary>
        ///値を画面に表示する
        /// </summary>
        /// <param name="values"></param>
        private void displayValues(List<List<int>> values)
        {
            int comboBoxCounter = 1;
            foreach (List<int> line in values)//列の数だけループ
            {
                for (int rowIndex = 0; rowIndex < line.Count; rowIndex++)//行の数だけループ
                {
                    var comboBox = this.FindName($"cmb{comboBoxCounter}") as ComboBox;
                    if (comboBox != null && 0 != line[rowIndex])//値が0 = ""でなければ
                    {
                        comboBox.ItemsSource = new List<int> { line[rowIndex] };
                        comboBox.IsEditable = false;
                        comboBox.IsDropDownOpen = false;
                        comboBox.IsEnabled = false;
                        comboBox.SelectedIndex = 0; // 最初の項目を選択
                        comboBoxCounter++;
                    }
                    else if (comboBox != null)//“”なので値をセット
                    {
                        List<string> valuesToset = new List<string> { "", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
                        comboBox.ItemsSource = valuesToset;
                        comboBox.IsEditable = true;
                        comboBox.IsDropDownOpen = false;
                        comboBox.IsEnabled = true;
                        comboBox.SelectedIndex = 0; // 最初の項目を選択
                        comboBoxCounter++;
                    }
                }
            }
        }
        /// <summary>
        /// 初期化
        /// </summary>
        private void InitializeData()
        {
            int comboBoxCounter = 1;
            for (int columnIndex = 0; columnIndex < 9; columnIndex++)//列の数だけループ
            {
                for (int rowIndex = 0; rowIndex < 9; rowIndex++)//行の数だけループ
                {
                    var comboBox = this.FindName($"cmb{comboBoxCounter}") as ComboBox;
                    if (comboBox != null)//“”なので値をセット
                    {
                        List<string> valuesToset = new List<string> { "", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
                        comboBox.ItemsSource = valuesToset;
                        comboBox.IsEditable = true;
                        comboBox.IsDropDownOpen = false;
                        comboBox.IsEnabled = true;
                        comboBox.SelectedIndex = -1; // 最初の項目を選択
                        comboBoxCounter++;
                    }
                }
            }
        }
        /// <summary>
        /// 初期化ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnInitializeData_Click(object sender, RoutedEventArgs e)
        {
            this.InitializeData();
            this.BtnClearData.IsEnabled = false;
            this.BtnComfirmData.IsEnabled = false;
        }
        /// <summary>
        ///     やり直しボタンのクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClearData_Click(object sender, RoutedEventArgs e)
        {
            List<List<int>> values = new List<List<int>>();
            //読み込んだ値をリセットしてユーザーから入力された値をクリア
            if (ControlFile.ReadFile(this.filePath, ref values)
                == true)
            {
                displayValues(values);
                this.BtnClearData.IsEnabled = true;
                this.BtnComfirmData.IsEnabled = true;
            }
        }
        /// <summary>
        /// 確認するボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnComfirmData_Click(object sender, RoutedEventArgs e)
        {
            const int checkIfAllDataAreSelected = 1;
            const int checkIfAllRowDataAreUnique = 2;
            const int checkIfAllcolumnDataAreUnique = 3;
            const int checkIfAll3x3DataAreUnique = 4;
            //すべてのデータがにゅうりょくされているかチェック
            if (this.RoopCmbDrectionRow(checkIfAllDataAreSelected) == false) 
            {
                return;
            }
            //すべての行データがユニークかチェック
            if (this.RoopCmbDrectionRow(checkIfAllRowDataAreUnique) == false)
            {
                return;
            }
            //すべての列データがユニークかチェック
            if (this.RoopCmbDrectionColumn(checkIfAllcolumnDataAreUnique) == false)
            {
                return;
            }
            //すべての3x3のデータがユニークかチェック
            if (this.RoopCmbDrection3x3(checkIfAll3x3DataAreUnique) == false)
            {
                return;
            }
            MessageBox.Show("正解です");
        }

        /// <summary>
        /// ループを実行するための関数 行　評価用　
        /// 
        /// comand 1 = すべての値が入力されているかチェック
        /// comand 2 = 行のデータがユニークかチェック
        /// comand 3 = 列のデータがユニークかチェック
        /// comand 4 = 3x3のデータがユニークかチェック
        /// </summary>
        /// <param name="comand">コマンドによって処理を分岐</param>
        private bool RoopCmbDrectionRow(int comand)
        {
            List<int> numbers = new List<int>();///番号
            int comboBoxCounter = 1;//コンボボックスのカウンター
            for (int columnIndex = 0; columnIndex < 9; columnIndex++)//列の数だけループ
            {
                for (int rowIndex = 0; rowIndex < 9; rowIndex++)//行の数だけループ
                {
                    if (this.SwitchForComand(comand, comboBoxCounter, numbers) == false) 
                    {
                        return false;
                    }
                    comboBoxCounter++;
                }
                numbers = new();//9個データを評価したらリストを初期化
            }
            return true;
        }
        /// <summary>
        /// ループを実行するための関数 列　評価用
        /// 
        /// comand 1 = すべての値が入力されているかチェック
        /// comand 2 = 行のデータがユニークかチェック
        /// comand 3 = 列のデータがユニークかチェック
        /// comand 4 = 3x3のデータがユニークかチェック
        /// </summary>
        /// <param name="comand">コマンドによって処理を分岐</param>
        /// <returns></returns>
        private bool RoopCmbDrectionColumn(int comand)
        {
            const int comboDirectionSwitcher = 9;//行番号の移動に使うための定数
            const int rowIndexMaxCount = 9;//行の最大個数
            const int columnIndexMaxCount = 9;//列の最大個数
            List<int> numbers = new List<int>();///番号
            int comboBoxCounter = 1;//コンボボックスのカウンター
            for (int rowIndex = 1; rowIndex <= rowIndexMaxCount; rowIndex++)//列の数だけループ
            {
                comboBoxCounter = rowIndex;
                for (int columnIndex = 1; columnIndex <= columnIndexMaxCount; columnIndex++)//行の数だけループ
                {
                    if (this.SwitchForComand(comand, comboBoxCounter, numbers) == false)
                    {
                        return false;
                    }
                    comboBoxCounter+= comboDirectionSwitcher;
                }
                numbers = new();//9個データを評価したらリストを初期化
            }
            return true;
        }

        /// <summary>
        /// ループを実行するための関数 3x3　評価用
        /// 
        /// comand 1 = すべての値が入力されているかチェック
        /// comand 2 = 行のデータがユニークかチェック
        /// comand 3 = 列のデータがユニークかチェック
        /// comand 4 = 3x3のデータがユニークかチェック
        /// 
        /// </summary>
        /// <param name="comand">コマンドによって処理を分岐</param>
        /// <returns></returns>
        private bool RoopCmbDrection3x3(int comand)
        {
            const int maxValuesCount = 81;//総データ数
            const int valuesCountof3Blocks = 27;//3ブロック文のデータ数
            const int maxCountOfValuesInABlock = 9;//1ブロックのデータ数
            const int nextComboBoxPosition = 17;//次のブロックの一番目のポジションを特定するのに17を減算するのに使う
            int lastRowIndex = 0;
            int rowIndex = 0;
            int resetCounter = 1;///データをが9こになったらリセットする(初期値は1)
            List<int> numbers = new List<int>();///番号
            int comboBoxPosition = 1;//コンボボックスのカウンター
            for (int index = 1; index <= maxValuesCount; index++)//列の数だけループ
            {
                //  コンボボックスの位置を指定して値の重複評価
                if (this.SwitchForComand(comand, comboBoxPosition, numbers) == false)
                {
                    return false;
                }
                ///27個(3ブロックのデータの評価が終わったら...)
                if (comboBoxPosition == valuesCountof3Blocks || comboBoxPosition == valuesCountof3Blocks * 2)
                {
                    resetCounter = 1;//カウンター1でをリセット
                    rowIndex = 0;
                    comboBoxPosition++;
                    numbers = new();
                    continue;
                }
                ///9個(1ブロックのデータの評価が終わったら...)
                if (resetCounter == maxCountOfValuesInABlock)
                {
                    resetCounter = 1;//カウンター1でをリセット
                    lastRowIndex = rowIndex;
                    rowIndex = lastRowIndex + 1;
                    comboBoxPosition = comboBoxPosition - nextComboBoxPosition;
                    numbers = new();
                }
                //ブロック内で行が3つ目まで評価が終わったら...
                else if (rowIndex == 2 || rowIndex == 5 || rowIndex == 8)
                {
                    lastRowIndex = rowIndex;
                    resetCounter++;
                    rowIndex = lastRowIndex - 2;//2row前の値を代入
                    comboBoxPosition += 7;//コンボボックスの番号を7個繰り上げる
                }
                else 
                {
                    comboBoxPosition += 1;
                    lastRowIndex = rowIndex;
                    resetCounter++;
                    rowIndex++;
                }
            }
            return true;
        }

        /// <summary>
        /// コマンドによって分岐するための関数
        /// </summary>
        /// <param name="comand">コマンド番号</param>
        /// <param name="comboBoxPosition">コンボボックスの位置</param>
        /// <param name="numbers">データリスト　最大9個</param>
        private bool SwitchForComand(int comand, int comboBoxPosition, List<int> numbers) 
        {
            switch (comand)
            {
                case 1:
                    if (CheckIfAlldatAreInserted(comboBoxPosition) == false) 
                    {
                        return false;
                    }
                    break;
                case 2:

                    if (CheckIfAllDataAreUnique(comboBoxPosition, numbers) == false) 
                    {
                        return false;
                    }
                    break;
                case 3:
                    if (CheckIfAllDataAreUnique(comboBoxPosition, numbers) == false)
                    {
                        return false;
                    }
                    break;
                case 4:
                    if (CheckIfAllDataAreUnique(comboBoxPosition, numbers) == false)
                    {
                        return false;
                    }
                    break;
            }
            return true;
        }
        /// <summary>
        /// すべてのコンボボックスの値が"すべて"入力されたかチェック
        /// </summary>
        /// <param name="comboBoxPosition"></param>
        private bool CheckIfAlldatAreInserted (int comboBoxPosition)
        {
            var comboBox = this.FindName($"cmb{comboBoxPosition}") as ComboBox;
            if (comboBox != null && comboBox.SelectedValue.Equals(""))//値が0 = ""でなければ
            {
                MessageBox.Show("値が入力されていない項目があります");
                comboBox.Focus();
                return false;
            }
            return true;
        }
        /// <summary>
        /// コンボボックスのデータがユニークかチェック
        /// </summary>
        /// <param name="comboBoxPosition">コンボボックスの位置</param>
        /// <param name="numbers">データリスト　最大9個</param>
        /// <returns></returns>
        private bool CheckIfAllDataAreUnique(int comboBoxPosition, List<int> numbers) 
        {
            var comboBox = this.FindName($"cmb{comboBoxPosition}") as ComboBox;
            if (comboBox != null)//値が0 = ""でなければ
            {
                string value = comboBox.SelectedValue.ToString();
                int num = int.Parse(value);
                ///同じ番号が存在すれば
                if (ControlFile.ContainsSameNumberInList(num, numbers))
                {
                    MessageBox.Show("同じデータが確認されました");
                    comboBox.Focus();
                    return false;
                }
                numbers.Add(num);
            }
            return true;
        }
    }
    ////////////////////////////////////////////////////////////////////////////////////
    //
    // イベントハンドラ処理
    ////////////////////////////////////////////////////////////////////////////////////
    // TODO Githubを理解する為のコメントを追加6月12日12時50分

    /// <summary>
    /// イベント一括登録
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // ComboBoxクラスに対してPreviewKeyDownイベントを一括登録
            EventManager.RegisterClassHandler(
                typeof(ComboBox),
                UIElement.PreviewKeyDownEvent,
                new KeyEventHandler(ComboBox_PreviewKeyDown),
                handledEventsToo: true);
        }
        /// <summary>
        /// すべてのコンボボックスに以下の制約をかける
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.IsEditable) //Null以外で編集可能であれば
            {
                string text = comboBox.Text;
                if (text.Length > 0 && e.Key != Key.Back)//テキストが2回目に入力されることを阻止、
                                                         //バックスペースキーのみ許す
                {
                    e.Handled = true;
                    return;
                }
                if ((e.Key < Key.D1 || e.Key > Key.D9) &&//1～9までの数字以外の文字入力を阻止
                    (e.Key < Key.NumPad1 || e.Key > Key.NumPad9) &&
                    e.Key != Key.Back)
                {
                    e.Handled = true;
                }
            }
        }
    }
}