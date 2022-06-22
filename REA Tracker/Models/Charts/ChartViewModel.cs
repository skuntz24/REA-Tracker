using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI.DataVisualization.Charting;
using QVICommonIntranet.Database;

namespace REA_Tracker.Models
{
    public class ChartViewModel
    {
        public class PieChart
        {
            public Chart DisplayChart { get; set; }

            public PieChart(String Title, String ArgsxValues, String ArgsyValues, String ArgsMetrics, int? ParamWidth, int? ParamHeight)
            {
                String[] yCount = ArgsyValues.Split(',');
                String[] yMetric = ArgsMetrics.Split(',');

                String[] yaxis = new String[yCount.Length];
                for (int i = 0; i < yCount.Length; i++)
                {
                    yaxis[i] = yMetric[i] + "[" + yCount[i] + "]";
                }
                Chart tempChart = new Chart();
                String[] xaxis;
                xaxis = new[] { "Low: " + yaxis[0], "Medium: " + yaxis[1], "High: " + yaxis[2], "Critical: " + yaxis[3] };

                String label = ArgsxValues;
                tempChart.Series.Add(new Series(label));
                tempChart.ChartAreas.Add(new ChartArea(label));
                tempChart.Series[label].ChartArea = label;
                tempChart.Series[label].ChartType = SeriesChartType.Pie;

                //tempChart.Series[label]["PieLabelStyle"] = "Outside";
                //tempChart.Series[label]["PieLineColor"] = "Black";
                tempChart.Series[label].YValuesPerPoint = 2;
                tempChart.Series[label].Points.DataBindXY(
                        xaxis, label,
                        yMetric, "#VALY"
                    //new[]{"1","2","3","4"}, "B"
                    );
                /*
                if (ArgsxValues == "Total")
                {
                    tempChart.Series[label].Label = "#VALY";
                }
                else if (ArgsxValues == "Problem") 
                {
                    tempChart.Series[label]["PieLabelStyle"] = "Outside";
                    tempChart.Series[label]["PieLineColor"] = "Black";
                    tempChart.Series[label].Label = "#VALY";
                }
                else
                {}
                 */

                tempChart.Series[label]["PieLabelStyle"] = "Disabled";

                tempChart.Palette = ChartColorPalette.None;
                tempChart.PaletteCustomColors = new[] { Color.DodgerBlue, Color.LimeGreen, Color.Yellow, Color.IndianRed };
                tempChart.ChartAreas[label].Name = label;
                tempChart.Titles.Add(label);
                tempChart.Titles[0].DockedToChartArea = label;
                tempChart.Titles[0].Alignment = ContentAlignment.TopCenter;
                tempChart.Titles[0].Docking = Docking.Top;
                tempChart.Titles[0].Position.X = 50;
                tempChart.Titles[0].Position.Y = 0;
                tempChart.ChartAreas[label].Position.X = 10;
                tempChart.ChartAreas[label].Position.Y = 10;
                tempChart.ChartAreas[label].Position.Height = 70;
                tempChart.ChartAreas[label].Position.Width = 75;

                tempChart.ChartAreas[label].IsSameFontSizeForAllAxes = true;
                tempChart.ChartAreas[label].Area3DStyle.Enable3D = true;

                tempChart.Legends.Add(new Legend("Legend"));
                tempChart.Legends["Legend"].Docking = Docking.Bottom;
                tempChart.Legends["Legend"].Alignment = StringAlignment.Center;

                tempChart.Series[label].Legend = "Legend";
                LegendCellColumn firstColumn = new LegendCellColumn();
                firstColumn.ColumnType = LegendCellColumnType.SeriesSymbol;
                firstColumn.MaximumWidth = 100;
                LegendCellColumn secondColumn = new LegendCellColumn();
                secondColumn.ColumnType = LegendCellColumnType.Text;

                secondColumn.Text = "#AXISLABEL";
                secondColumn.Alignment = ContentAlignment.MiddleLeft;
                tempChart.Legends["Legend"].ItemColumnSpacing = 250;

                //tempChart.Legends["Legend"].AutoFitMinFontSize = true;

                tempChart.Legends["Legend"].CellColumns.Add(firstColumn);
                tempChart.Legends["Legend"].CellColumns.Add(secondColumn);

                tempChart.Width = ParamWidth == null ? 300 : (int)ParamWidth;
                tempChart.Height = ParamHeight == null ? 210 : (int)ParamHeight;
                DisplayChart = tempChart;
            }
            /*
            public PieChart(String Title, String ArgsxValues, String ArgsyValues, int? ParamWidth, int? ParamHeight) 
            {
                if(ArgsyValues.Length>0)
                {
                    String[] parsedX = ArgsxValues.Split(',');
                    String[][] parsedY = deparse(ArgsyValues);
                    Chart tempChart = new Chart();
                    String[] xaxis = {"Low","Medium","High","Critical"};
                    for(int x =0; x<parsedX.Length; x++)
                    {
                        String label = parsedX[x];
                        String[] yaxis = parsedY[x];
                        tempChart.Series.Add(new Series(label));
                        tempChart.ChartAreas.Add(new ChartArea(label));
                        tempChart.Series[label].ChartArea = label;
                        tempChart.Series[label].ChartType = SeriesChartType.Pie;
                        
                        tempChart.Series[label]["PieLabelStyle"] = "Outside";
                        tempChart.Series[label]["PieLineColor"] = "Black";
                         
                        tempChart.Series[label].Points.DataBindXY(
                                xaxis, label,
                                yaxis, "Number of Instances"
                                //new[]{"1","2","3","4"}, "B"
                            );
                        tempChart.Series[label].IsValueShownAsLabel = true;
                        tempChart.Palette = ChartColorPalette.None;
                        tempChart.PaletteCustomColors = new[]{ Color.DodgerBlue ,Color.LimeGreen,Color.Yellow, Color.IndianRed };
                        tempChart.ChartAreas[label].Name = label;
                        tempChart.Titles.Add(label);
                        tempChart.Titles[x].DockedToChartArea = label;
                        tempChart.ChartAreas[label].Position.X = (100 / parsedX.Length)*x;
                        tempChart.ChartAreas[label].Position.Y = 0;
                        tempChart.ChartAreas[label].Position.Height = 50;
                        tempChart.ChartAreas[label].Position.Width = (100 / parsedX.Length);
                    }
                    tempChart.Legends.Add("Default");
                    tempChart.Legends["Default"].HeaderSeparator = LegendSeparatorStyle.Line;

                    // Add Color column      
                    LegendCellColumn firstColumn = new LegendCellColumn();
                    firstColumn.ColumnType = LegendCellColumnType.SeriesSymbol;
                    firstColumn.HeaderText = "Color";
                    firstColumn.HeaderBackColor = Color.WhiteSmoke;
                    tempChart.Legends["Default"].CellColumns.Add(firstColumn);

                    tempChart.Height = 600;
                    tempChart.Width = 1000;
                    DisplayChart = tempChart;
                }
            }
            */
            private String[][] deparse(String yinput)
            {//rows are: total, problem, enhancement, then planned work
             //Columns are Low, medium, high, critical
                String[] rowY = yinput.Split(';');
                List<String[]> results = new List<String[]>();
                for (int i = 0; i < rowY.Length; i++)
                {
                    results.Add(rowY[i].Split(','));
                }
                return results.ToArray();
            }
            public string FormmatArrayToString(String[][] param)
            {
                String results = "";
                for (int i = 0; i < param.Length; i++)
                {
                    if (i != 0)
                    {
                        results += ";";
                    }
                    for (int j = 0; j < param[i].Length; j++)
                    {
                        if (j != 0)
                        {
                            results += ",";
                        }
                        results += (param[i][j].ToString());
                    }
                }
                return results;
            }
        }
    }

    public class CustomChartViewModel
    {
        public class CustomPieChart
        {
            public Chart DisplayChart { get; set; }

            public CustomPieChart(string Title, string ArgsxValues, string ArgsyValues, int colorenum, int? ParamWidth, int? ParamHeight)
            {//X values are the labels
             //Y values are the data sets

                string[] yCount = ArgsyValues == null ? new string[] { } : ArgsyValues.Split(',');
                string[] xCount = ArgsxValues == null ? new string[] { } : ArgsxValues.Split(',');

                string[] yaxis = new String[yCount.Length];
                string[] xaxis = new String[xCount.Length];
                for (int i = 0; i < yCount.Length; i++)
                {
                    yaxis[i] = yCount[i] + " [" + xCount[i] + "] #PERCENT{P2}";
                    xaxis[i] = xCount[i];
                }


                Chart tempChart = new Chart();
                tempChart.Series.Add(new Series(Title));
                tempChart.ChartAreas.Add(new ChartArea(Title));
                tempChart.Series[Title].ChartArea = Title;
                tempChart.Series[Title].ChartType = SeriesChartType.Pie;

                tempChart.Series[Title].YValuesPerPoint = 1;

                tempChart.Series[Title].Points.DataBindXY(

                    yaxis, "#VALY"
                    , xaxis, Title
                    );
                tempChart.Series[Title]["PieLabelStyle"] = "Disabled";

                tempChart.Palette = (ChartColorPalette.None);

                tempChart.PaletteCustomColors = Getcolor(colorenum);
                /*
                tempChart.Series[Title]["PieLabelStyle"] = "Inside";
                tempChart.Series[Title]["PieLineColor"] = "Black";

                tempChart.Series[Title].Label = "";
                */

                tempChart.ChartAreas[Title].Name = Title;
                tempChart.Titles.Add(Title);
                tempChart.Titles[0].DockedToChartArea = Title;
                tempChart.Titles[0].Alignment = ContentAlignment.TopCenter;
                tempChart.Titles[0].Docking = Docking.Top;
                tempChart.Titles[0].Position.X = 50;
                tempChart.Titles[0].Position.Y = 0;
                tempChart.ChartAreas[Title].Position.X = 10;
                tempChart.ChartAreas[Title].Position.Y = 10;
                tempChart.ChartAreas[Title].Position.Height = 50;
                tempChart.ChartAreas[Title].Position.Width = 75;

                tempChart.ChartAreas[Title].IsSameFontSizeForAllAxes = true;
                tempChart.ChartAreas[Title].Area3DStyle.Enable3D = true;

                tempChart.Legends.Add(new Legend("Legend"));
                tempChart.Legends["Legend"].Docking = Docking.Bottom;
                tempChart.Legends["Legend"].Alignment = StringAlignment.Center;

                tempChart.Series[Title].Legend = "Legend";
                LegendCellColumn firstColumn = new LegendCellColumn();
                firstColumn.ColumnType = LegendCellColumnType.SeriesSymbol;
                firstColumn.MaximumWidth = 100;
                LegendCellColumn secondColumn = new LegendCellColumn();
                secondColumn.ColumnType = LegendCellColumnType.Text;

                secondColumn.Text = "#AXISLABEL";
                secondColumn.Alignment = ContentAlignment.MiddleLeft;
                tempChart.Legends["Legend"].ItemColumnSpacing = 250;

                //tempChart.Legends["Legend"].AutoFitMinFontSize = true;

                tempChart.Legends["Legend"].CellColumns.Add(firstColumn);
                tempChart.Legends["Legend"].CellColumns.Add(secondColumn);

                tempChart.Width = ParamWidth == null ? 200 : (int)ParamWidth;
                tempChart.Height = ParamHeight == null ? 350 : (int)ParamHeight;


                DisplayChart = tempChart;
            }

            private Color[] Getcolor(int colorEnum)
            {
                Color[] result = new[] { Color.DodgerBlue, Color.LimeGreen, Color.Yellow,
                    Color.IndianRed, Color.Orange, Color.Purple };
                switch (colorEnum)
                {
                    case (1):
                        result = new Color[]
                        {//Blue
                            Color.FromArgb(0,0,255),
                            Color.FromArgb(0,200,200),
                            Color.FromArgb(0,150,150),
                            Color.FromArgb(0,100,100),
                            Color.FromArgb(0,150,255),
                            Color.FromArgb(0,100,255),
                            Color.FromArgb(0,255,255),
                            Color.FromArgb(150,150,255)
                            /*
                            Color.DarkBlue,
                            Color.CornflowerBlue,
                            Color.DeepSkyBlue,
                            Color.DodgerBlue,
                            Color.DarkSlateBlue,
                            Color.CadetBlue*/
                        };
                        break;
                    case (2):
                        result = new Color[]
                        {//Green
                            Color.DarkGreen,
                            Color.DarkOliveGreen,
                            Color.LightGreen,
                            Color.LimeGreen,
                            Color.PaleGreen,
                            Color.LightSeaGreen
                        };
                        break;
                    case (3):
                        result = new Color[]
                        {//Yellow
                            Color.FromArgb(200,200,0),
                            Color.FromArgb(150,150,0),
                            Color.FromArgb(100,100,0),
                            Color.FromArgb(255,255,100),
                            Color.FromArgb(255,255,0),
                            Color.FromArgb(200,200,100),
                            Color.FromArgb(200,150,100),
                            Color.FromArgb(150,200,100)
                            /*
                            Color.GreenYellow,
                            Color.Yellow,
                            Color.LightYellow,
                            Color.YellowGreen,
                            Color.LightGoldenrodYellow
                             * */
                        };
                        break;
                    case (4):
                        result =
                            new Color[]
                            {//Red
                                Color.DarkRed,
                                Color.IndianRed,
                                Color.HotPink,
                                Color.PaleVioletRed,
                                Color.MediumVioletRed
                            };
                        break;
                    case (5):
                        result = new Color[]
                        {//Orange
                            Color.FromArgb(196,98,16),
                            Color.FromArgb(205,133,63),
                            Color.FromArgb(255,153,102),
                            Color.FromArgb(245,128,37),
                            Color.FromArgb(255,179,71),
                            Color.FromArgb(233,105,44),
                            Color.FromArgb(255,110,74)
                            /*
                            Color.DarkOrange,
                            Color.Orange,
                            Color.OrangeRed,
                            Color.Ivory,
                            Color.Gainsboro,
                            Color.Gray
                             */
                        };
                        break;
                    case (6):
                        result = new Color[]
                        {//Purple
                            Color.MediumPurple,
                            Color.Purple,
                            Color.Violet,
                            Color.Lavender,
                            Color.DarkViolet,
                            Color.DarkViolet
                        };
                        break;
                    default:
                        break;
                }
                return result;
            }

            private String[][] deparse(String yinput)
            {//rows are: total, problem, enhancement, then planned work
             //Columns are Low, medium, high, critical
                String[] rowY = yinput.Split(';');
                List<String[]> results = new List<String[]>();
                for (int i = 0; i < rowY.Length; i++)
                {
                    results.Add(rowY[i].Split(','));
                }
                return results.ToArray();
            }
            public string FormmatArrayToString(String[][] param)
            {
                String results = "";
                for (int i = 0; i < param.Length; i++)
                {
                    if (i != 0)
                    {
                        results += ";";
                    }
                    for (int j = 0; j < param[i].Length; j++)
                    {
                        if (j != 0)
                        {
                            results += ",";
                        }
                        results += (param[i][j].ToString());
                    }
                }
                return results;
            }
        }
    }
}