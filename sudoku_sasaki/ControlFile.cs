using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace sudoku_sasaki
{
    internal class ControlFile
    {
        /// <summary>
        /// ブロック内の値の数 (定数)
        /// </summary>
        private const int maxCountOfValuesInABlock = 9;
        /// <summary>
        /// 行の最大の座標値(定数)
        /// </summary>
        private const int columnMaxPosition = 8;
        /// <summary>
        /// 列の最大の座標値(定数)
        /// </summary>
        private const int rowMaxPosition = 8;
        /// <summary>
        /// ブロックの数
        /// </summary>
        private const int blockMaxCount = 9;

        /// <summary>
        /// 値を保存しておくためのリスト
        /// </summary>
        private static List<List<int>> values = new List<List<int>>()
        {
            new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0}
    };

        /// <summary>
        /// CSVファイルの読み込み
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="valuesForRef">値</param>
        public static bool ReadFile(string filePath, ref List<List<int>> valuesForRef)
        {
            List<string> lines = new();
            //List<string> lines = new List<string>();
            // ファイルの存在チェック
            if (!File.Exists(filePath))
            {
                return false;
            }
            try
            {
                using (StreamReader reader = new(filePath))
                {
                    string line;
                    // ファイルを1行ずつ読み込み
                    while ((line = reader.ReadLine()) != null)
                    {
                        // 各行をリストに追加
                        lines.Add(line);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"エラー: {ex.Message}");
                return false;
            }
            string errorMessage = ""; //ファイルの中に不正があればメッセージを付加する
            ///③読み込んだcsvファイルのデータが9×9の配列範囲であることをチェックする
            if (CheckIfFileFormatIsOK(lines, ref errorMessage) == false)
            {
                MessageBox.Show(errorMessage);
                return false;
            }
            //すべての横の行の数字がユニークかチェック
            for (int columnIndex = 0; columnIndex < ControlFile.values[columnIndex].Count - 1; columnIndex++)
            {
                if (!ControlFile.IsUniqueRowValues(columnIndex))
                {
                    MessageBox.Show("行の数値が重複しています");
                    return false;
                }
            }
            //すべての縦の列の数字がユニークかチェック
            for (int rowIndex = 0; rowIndex < ControlFile.values.Count; rowIndex++)
            {
                if (!ControlFile.IsUniqueColumnValues(rowIndex))
                {
                    MessageBox.Show("列の数値が重複しています");
                    return false;
                }
            }
            //3x3のデータがユニークかチェック
            if (!ControlFile.IsUnique3x3Values())
            {
                MessageBox.Show("3x3の数値が重複しています");
                return false;
            }
            valuesForRef = ControlFile.values;///値の引数への代入
            return true;
        }
        /// <summary>
        /// 読み込んだcsvファイルのデータ適切であるかのチェックを行う
        /// </summary>
        /// <param name="lines"></param>
        /// <returns>true = フォーマットが適切, false = フォーマットが不適切</returns>
        private static bool CheckIfFileFormatIsOK(List<string> lines, ref string errorMessage)
        {
            int columnIndex = 0;
            foreach (string line in lines)
            {
                string[] data = line.Split(',');///カンマで区切って文字列を分ける
                if (CheckIfDetaFormatIs9x9(data, lines, ref errorMessage) == false)
                {
                    return false;
                }
                ///データの値が正しいかどうかの判断
                if (IsInRange(data, ref errorMessage, columnIndex) == false)
                {
                    return false;
                }
                columnIndex++;
            }
            return true;
        }
        /// <summary>
        /// ③読み込んだcsvファイルのデータが9×9の配列範囲であることをチェックする
        /// </summary>
        /// <param name="data">81個のデータ</param>
        /// <param name="lines">(行)9つのデータ</param>
        /// <returns></returns>
        private static bool CheckIfDetaFormatIs9x9(string[] data, List<string> lines, ref string errorMessage)
        {
            if (data.Length == 9 && lines.Count == 9)
            {
                return true;
            }
            else
            {
                errorMessage = "データの配列範囲が9x9ではありません";
                return false;
            }
        }
        /// <summary>
        /// ④csvファイルから読み込んだ値が範囲内の値であることをチェックする
        /// </summary>
        /// <param name="data">81個のデータ</param>
        /// <param name="errorMessage">エラーメッセージ</param>
        /// <param name="columnIndex">列の配列番号</param>
        /// <returns>true = 範囲内, false = 範囲外</returns>
        private static bool IsInRange(string[] data, ref string errorMessage, int columnIndex)
        {
            int rowIndex = 0;
            foreach (string value in data)
            {
                bool isInteger = int.TryParse(value, out int result);
                if (isInteger)//数値かどうか
                {
                    if (result > 9)
                    {
                        errorMessage = "9より大きい値が含まれています";
                        return false;
                    }
                    ControlFile.values[columnIndex][rowIndex] = result;//数値であればリストにデータを代入する
                    rowIndex++;
                    continue;
                }
                else if (value.Contains(" "))
                {
                    ControlFile.values[columnIndex][rowIndex] = result;//スペースであればリストに0を代入する
                    rowIndex++;
                    continue;
                }
                else if (String.IsNullOrEmpty(value))
                {
                    ControlFile.values[columnIndex][rowIndex] = result;//空欄であればリストに0を代入する
                    rowIndex++;
                    continue;
                }
                else
                {
                    errorMessage = "数値ではない値が含まれています";
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 行の数値に重複があるかをチェック
        /// </summary>
        /// <param name="columnIndex">列の配列番号</param>
        /// <returns>true = 重複なし, false = 重複あり</returns>
        private static bool IsUniqueRowValues(int columnIndex)
        {
            List<int> numbers = new List<int>();///取り出した値を評価するためにためておくリスト
            foreach (int value in ControlFile.values[columnIndex])
            {
                switch (value)
                {
                    case 0:
                        //特に何もしない
                        continue;
                    case 1:
                        if (ControlFile.ContainsSameNumberInList(value, numbers))
                        {
                            return false;
                        }
                        numbers.Add(value);
                        break;
                    case 2:
                        if (ControlFile.ContainsSameNumberInList(value, numbers))
                        {
                            return false;
                        }
                        numbers.Add(value);
                        break;
                    case 3:
                        if (ControlFile.ContainsSameNumberInList(value, numbers))
                        {
                            return false;
                        }
                        numbers.Add(value);
                        break;
                    case 4:
                        if (ControlFile.ContainsSameNumberInList(value, numbers))
                        {
                            return false;
                        }
                        numbers.Add(value);
                        break;
                    case 5:
                        if (ControlFile.ContainsSameNumberInList(value, numbers))
                        {
                            return false;
                        }
                        numbers.Add(value);
                        break;
                    case 6:
                        if (ControlFile.ContainsSameNumberInList(value, numbers))
                        {
                            return false;
                        }
                        numbers.Add(value);
                        break;
                    case 7:
                        if (ControlFile.ContainsSameNumberInList(value, numbers))
                        {
                            return false;
                        }
                        numbers.Add(value);
                        break;
                    case 8:
                        if (ControlFile.ContainsSameNumberInList(value, numbers))
                        {
                            return false;
                        }
                        numbers.Add(value);
                        break;
                    case 9:
                        if (ControlFile.ContainsSameNumberInList(value, numbers))
                        {
                            return false;
                        }
                        numbers.Add(value);
                        break;
                }
            }
            return true;
        }
        /// <summary>
        //列の数値に重複があるかをチェック
        /// </summary>
        /// <param name="rowIndex">行の配列番号</param>
        /// <returns>true = 重複なし, false = 重複あり</returns>
        private static bool IsUniqueColumnValues(int rowIndex)
        {
            int value = 0;
            List<int> numbers = new List<int>();
            for (int columnIndex = 0; columnIndex < ControlFile.values[rowIndex].Count; columnIndex++)
            {
                value = ControlFile.values[columnIndex][rowIndex];
                switch (value)
                {
                    case 0:
                        //特に何もしない
                        continue;
                    case 1:
                        if (ControlFile.ContainsSameNumberInList(value, numbers))
                        {
                            return false;
                        }
                        numbers.Add(value);
                        break;
                    case 2:
                        if (ControlFile.ContainsSameNumberInList(value, numbers))
                        {
                            return false;
                        }
                        numbers.Add(value);
                        break;
                    case 3:
                        if (ControlFile.ContainsSameNumberInList(value, numbers))
                        {
                            return false;
                        }
                        numbers.Add(value);
                        break;
                    case 4:
                        if (ControlFile.ContainsSameNumberInList(value, numbers))
                        {
                            return false;
                        }
                        numbers.Add(value);
                        break;
                    case 5:
                        if (ControlFile.ContainsSameNumberInList(value, numbers))
                        {
                            return false;
                        }
                        numbers.Add(value);
                        break;
                    case 6:
                        if (ControlFile.ContainsSameNumberInList(value, numbers))
                        {
                            return false;
                        }
                        numbers.Add(value);
                        break;
                    case 7:
                        if (ControlFile.ContainsSameNumberInList(value, numbers))
                        {
                            return false;
                        }
                        numbers.Add(value);
                        break;
                    case 8:
                        if (ControlFile.ContainsSameNumberInList(value, numbers))
                        {
                            return false;
                        }
                        numbers.Add(value);
                        break;
                    case 9:
                        if (ControlFile.ContainsSameNumberInList(value, numbers))
                        {
                            return false;
                        }
                        numbers.Add(value);
                        break;
                }
            }
            return true;
        }
        /// <summary>
        /// Listの中に同じ番号がないかチェック
        /// </summary>
        /// <param name="item">値</param>
        /// <param name="numbers">取り出した値を評価するためにためておくリスト、9個保存して同じ番号が
        /// 認められなければ newして使う</param>
        /// <returns>true = 同じ番号がある</returns>
        public static bool ContainsSameNumberInList(int item, List<int> numbers)
        {
            if (numbers.Contains(item))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        //各ブロックの数値がユニークかをチェック
        /// </summary>
        /// <returns></returns>
        private static bool IsUnique3x3Values()
        {
            int columnIndex = 0;//列の配列番号
            int rowIndex = 0;//行の配列番号
            int value = 0;//値を入れるための変数
            int roopIndex = 0;//ループを回すための番号
            int resetCounter = 1;///データが9こになったらリセットする(初期値は1)
            int totalCountOfValues = ControlFile.values.Count * ControlFile.values[rowIndex].Count;//総データ数
            int lastRowIndex = 0;//以前の行の配列番号
            int lastColumnIndex = 0;//以前の列の配列番号
            int valuesCountof3Blocks = 27;//3ブロックの値総数
            List<int> numbers = new List<int>();///番号
            //データの数だけループ処理
            while (roopIndex < totalCountOfValues)
            {
#if DEBUG
                // デバッグ時にのみ実行するコード
                Console.WriteLine("デバッグモードです");
                if (columnIndex == 2 && rowIndex == 8)
                {
                }
#endif
                value = ControlFile.values[columnIndex][rowIndex];
                switch (value)
                {
                    case 0:
                        //特に何もしない
                        break;
                    case 1:
                        if (ControlFile.ContainsSameNumberInList(value, numbers))
                        {
                            return false;
                        }
                        numbers.Add(value);
                        break;
                    case 2:
                        if (ControlFile.ContainsSameNumberInList(value, numbers))
                        {
                            return false;
                        }
                        numbers.Add(value);
                        break;
                    case 3:
                        if (ControlFile.ContainsSameNumberInList(value, numbers))
                        {
                            return false;
                        }
                        numbers.Add(value);
                        break;
                    case 4:
                        if (ControlFile.ContainsSameNumberInList(value, numbers))
                        {
                            return false;
                        }
                        numbers.Add(value);
                        break;
                    case 5:
                        if (ControlFile.ContainsSameNumberInList(value, numbers))
                        {
                            return false;
                        }
                        numbers.Add(value);
                        break;
                    case 6:
                        if (ControlFile.ContainsSameNumberInList(value, numbers))
                        {
                            return false;
                        }
                        numbers.Add(value);
                        break;
                    case 7:
                        if (ControlFile.ContainsSameNumberInList(value, numbers))
                        {
                            return false;
                        }
                        numbers.Add(value);
                        break;
                    case 8:
                        if (ControlFile.ContainsSameNumberInList(value, numbers))
                        {
                            return false;
                        }
                        numbers.Add(value);
                        break;
                    case 9:
                        if (ControlFile.ContainsSameNumberInList(value, numbers))
                        {
                            return false;
                        }
                        numbers.Add(value);
                        break;
                }
                ///3ブロックの評価が終わったら
                if (roopIndex == valuesCountof3Blocks - 1 || roopIndex == valuesCountof3Blocks * 2 - 1)
                {
                    resetCounter = 1;//カウンター1でをリセット
                    lastRowIndex = rowIndex;
                    rowIndex = 0;
                    columnIndex += 1;
                    roopIndex++;
                    numbers = new();
                    continue;
                }
                ///9個(1ブロックのデータの評価が終わったら...)
                if (resetCounter == ControlFile.maxCountOfValuesInABlock)
                {
                    resetCounter = 1;//カウンター1でをリセット
                    lastRowIndex = rowIndex;
                    lastColumnIndex = columnIndex;
                    columnIndex = lastColumnIndex - 2;//2 column前の値を代入
                    rowIndex = lastRowIndex + 1;
                    numbers = new();
                }
                /// 行の配列番号が2.5.8のときの挙動
                else if (rowIndex == 2 || rowIndex == 5 || rowIndex == 8)
                {
                    lastRowIndex = rowIndex;
                    lastColumnIndex = columnIndex;
                    resetCounter++;
                    columnIndex++;
                    rowIndex = lastRowIndex - 2;//2row前の値を代入
                }
                //それ以外の時の挙動
                else
                {
                    lastRowIndex = rowIndex;
                    lastColumnIndex = columnIndex;
                    resetCounter++;
                    rowIndex++;
                }
                roopIndex++;
            }
            return true;
        }
    }
}
