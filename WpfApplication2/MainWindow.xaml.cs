using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace WpfApplication2
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<ResInfo> myList;
        public string rbtCheck { get; set; }
        public class ResInfo
        {
            public string name { get; set; }
            public string menu { get; set; }
            public double distance { get; set; }
            public double taste { get; set; }
            public double price { get; set; }
            public double total { get; set; }
            public double period { get; set; }

            public ResInfo (string name, string menu, double distance, double taste, double price,double period)
            {
                this.name = name;
                this.menu = menu;
                this.distance = distance;
                this.taste = taste;
                this.price = price;
            }
            public ResInfo ()
            {
                // TODO: Complete member initialization
            }
        }
        public MainWindow ()
        {
            InitializeComponent();
            ReadExcelData("C:/Users/cyshin/Desktop/LunchGenerator/text.xlsx");
        }
        public void WriteExcelDate (string path, object[] mydata)
        {
            Excel.Application excelApp = null;
            Excel.Workbook wb = null;
            Excel.Worksheet ws = null;
            try {
                excelApp = new Excel.Application();
                wb = excelApp.Workbooks.Open(path);
                // path 대신 문자열도 가능합니다
                // 예. Open(@"D:\test\test.xslx");
                ws = wb.Worksheets.get_Item(1) as Excel.Worksheet;
                // 첫번째 Worksheet를 선택합니다.
                Excel.Range rng = ws.UsedRange;   // '여기'
                // 현재 Worksheet에서 사용된 셀 전체를 선택합니다.
                
                object[,] data = rng.Value;
                // 열들에 들어있는 Data를 배열 (One-based array)로 받아옵니다.
                 
                for (int i = 0; i < mydata.Length; i++) {
                    ws.Cells[data.GetLength(0)+1 , i+1] = mydata[i];
                }
                
                // data를 불러온 엑셀파일에 적용시킵니다. 아직 완료 X
                /*
                if (path != null)
                    // path는 새로 저장될 엑셀파일의 경로입니다.
                    // 따로 지정해준다면, "다른이름으로 저장" 의 역할을 합니다.
                    // 상대경로도 가능합니다. (예. "secondExcel.xlsx")
                    wb.SaveAs(path,Excel.XlFileFormat.xlWorkbookNormal);
                else
                    // 따로 저장하지 않는다면 지금 파일에 그대로 저장합니다.
                    wb.Save();
                */
                wb.Save();
             
                wb.Close(false,Type.Missing,Type.Missing);
                excelApp.Quit();

                return;
            } catch (Exception ex) {
                throw ex;
            } finally {
                ReleaseExcelObject(ws);
                ReleaseExcelObject(wb);
                ReleaseExcelObject(excelApp);
            }
        
        }        
        public void ReadExcelData (string path)
        { // path는 Excel파일의 전체 경로입니다.
            // 예. D:\test\test.xslx
            Excel.Application excelApp = null;
            Excel.Workbook wb = null;
            Excel.Worksheet ws = null;
            try {
                excelApp = new Excel.Application(path);
                wb = excelApp.Workbooks.Open(path);
                // path 대신 문자열도 가능합니다
                // 예. Open(@"D:\test\test.xslx");
                ws = wb.Worksheets.get_Item(1) as Excel.Worksheet;
                // 첫번째 Worksheet를 선택합니다.
                Excel.Range rng = ws.UsedRange;   // '여기'
                // 현재 Worksheet에서 사용된 셀 전체를 선택합니다.
                myList = new List<ResInfo>();
                
                TimeSpan day = new TimeSpan();
                for (int i = 2; i <= ws.UsedRange.Rows.Count; i++) {

                    if (ws.Cells[i, 6].ToString() != null) {
                        day = DateTime.Now - Convert.ToDateTime(ws.Cells[i, 6].ToString());
                    }
                    if ((double)day.TotalDays <= 7) {
                        ws.Cells[i, 7] = "1";
                    } else if ((double)day.TotalDays > 7 && (double)day.TotalDays <= 14) {
                        ws.Cells[i, 7] = "2";
                    } else if ((double)day.TotalDays > 14 && (double)day.TotalDays <= 21) {
                        ws.Cells[i, 7] = "3";
                    } else if ((double)day.TotalDays > 21 && (double)day.TotalDays <= 28) {
                        ws.Cells[i, 7] = "4";
                    } else if ((double)day.TotalDays > 28) {
                        ws.Cells[i, 7] = "5";
                    }
                }

                object[,] data = rng.Value;

                // 열들에 들어있는 Data를 배열 (One-based array)로 받아옵니다.
                for (int r = 2; r <= data.GetLength(0); r++) {
                    ResInfo RI = new ResInfo(data[r, 1].ToString(), data[r, 2].ToString(),
                        double.Parse(data[r, 3].ToString()), double.Parse(data[r, 4].ToString()),
                        double.Parse(data[r, 5].ToString()), double.Parse(data[r, 7].ToString()));
                    myList.Add(RI);
                }
                tbxDate.Text = data[1,6].ToString();
                
                wb.Close(true);
                excelApp.Quit();

                return;
            } catch (Exception ex) {
                throw ex;
            } finally {
                ReleaseExcelObject(ws);
                ReleaseExcelObject(wb);
                ReleaseExcelObject(excelApp);
            }
        }
        private static void ReleaseExcelObject (object obj)
        {
            try {
                if (obj != null) {
                    Marshal.ReleaseComObject(obj);
                    obj = null;
                }
            } catch (Exception ex) {
                obj = null;
                throw ex;
            } finally {
                GC.Collect();
            }
        }
        public void getTotal (ResInfo RI, string mode)
        {
            double m_Taste = 1;
            double m_Distance = 1;
            double m_Price = 1;

            if (mode.Equals("맛")) {
                m_Taste = 4;
            } else if (mode.Equals("거리")) {
                m_Distance = 4;
            } else if (mode.Equals("가격")) {
                m_Price = 4;
            } else if (mode.Equals("랜덤")) {

            } else if (mode.Equals("올랜")) {
                //로직이 달라짐'
            }

            Random rand = new Random(DateTime.Now.Millisecond);
            System.Threading.Thread.Sleep(10);

            double a, b, c;
            int r_Taste = rand.Next(1, 101);
            int r_Distance = rand.Next(1, 101);
            int r_Price = rand.Next(1, 101);
            //tbxScore.Text += "r1 : " + r_Taste.ToString() + " r2 : " + r_Distance.ToString() + " r3 : " + r_Price.ToString() + "\n"; 
            a = fomula(m_Taste, RI.distance, r_Taste);
            b = fomula(m_Distance, RI.taste, r_Distance);
            c = fomula(m_Price, RI.price, r_Price);
            double total = a + b + c;
            RI.total = total;
        }
        public void refreshTotal (string mode)
        {
            foreach (ResInfo item in myList) {
                getTotal(item, mode);
            }
            return;
        }
        public double fomula (double x, double y, double z)    //weight, choice, random
        {
            return x / 5 * y / 5 * z / 100; 
        }
        public ResInfo findMax ()
        {
            ResInfo max = myList[0];

            foreach (ResInfo item in myList) {
                 if (item.total > max.total) {
                    max = item;
                }
            }
            return max;
        }
        public void showData (List<ResInfo> myList,ResInfo Max)
        {
            foreach (ResInfo item in myList) {
                tbxScore.Text += item.name + " | " + item.total.ToString() + "\n"; 
            }
            tbxName.Text += Max.name;
            tbxMenu.Text += Max.menu;
        }
        private void rbtRandom_Checked (object sender, RoutedEventArgs e)
        {
            rbtCheck = rbtRandom.Content.ToString();
        }
        private void rbtTaste_Checked (object sender, RoutedEventArgs e)
        {
            rbtCheck = rbtTaste.Content.ToString();
        }
        private void rbtDistance_Checked (object sender, RoutedEventArgs e)
        {
            rbtCheck = rbtDistance.Content.ToString();
        }
        private void rbtPrice_Checked (object sender, RoutedEventArgs e)
        {
            rbtCheck = rbtPrice.Content.ToString();
        }
        private void Button_Click_One (object sender, RoutedEventArgs e)
        {
            ResInfo Max;
            //find max
            refreshTotal(rbtCheck);
            Max = findMax();
            //show data
            CleanAllBox();
            showData(myList, Max);

        }
        private void Button_Click_Rank (object sender, RoutedEventArgs e)
        {
            List<ResInfo> RankList = new List<ResInfo>();
            //find max

            ResInfo Max;
            CleanAllBox();
            for (int i = 0; i < 5; i++) {
                refreshTotal(rbtCheck);
                Max = findMax();
                showData(myList, Max);
                ResInfo RI = new ResInfo();
                RI.name = Max.name;
                RI.menu = Max.menu;
                RI.total = Max.total;
                RankList.Add(RI);
            }
            int j = 1;
            foreach (ResInfo item in RankList) {
                tbxRank.Text += j++ + ". " + item.name + " " + item.menu + " " + item.total + "\n";
            }
        }
        private void CleanAllBox ()
        {
            tbxScore.Clear();
            tbxName.Clear();
            tbxMenu.Clear();
            tbxRank.Clear();
        }
        private object[,] UpdatePeriod (object[,] data, Excel.Worksheet ws)
        {
            
            
            return data;
        }
        private void Button_Click_Write (object sender, RoutedEventArgs e)
        {
            object[] data = new object[6];
            data[0] = tbxInName.Text;
            data[1] = tbxInMenu.Text;
            data[2] = tbxInDistance.Text;
            data[3] = tbxInTaste.Text;
            data[4] = tbxInPrice.Text;
            data[5] = tbxDate.Text;

            WriteExcelDate("C:/Users/cyshin/Desktop/LunchGenerator/text.xlsx",data);
            MessageBox.Show(" 입력되었습니다! ");
        }  
        /*
        private void rbtAllRan_Checked (object sender, RoutedEventArgs e)
        {
            rbtCheck = rbtAllRan.Content.ToString();
        }*/
    }
}
