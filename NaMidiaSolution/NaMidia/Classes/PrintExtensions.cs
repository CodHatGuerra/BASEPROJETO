using System.Collections.Generic;
using System.Linq;
using Telerik.Windows.Controls;
using System.Windows.Media;
using Telerik.Windows.Data;
using Telerik.Windows.Documents.Model;
using System.Collections;
using Telerik.Windows.Documents.FormatProviders.Pdf;
using Telerik.Windows.Documents.FormatProviders.Html;
using System.IO;
using System.ComponentModel;
using System.Windows;
using System;
using Microsoft.Win32;
using System.Windows.Controls;
using NaMidia.Classes;
using System.Windows.Media.Imaging;

namespace RadGridViewPrint
{
    public class PrintExportExtensions : DependencyObject, INotifyPropertyChanged
    {
        public PrintExportExtensions()
        {
            this.HeaderBackground = Color.FromArgb(255, 127, 127, 127);
            this.RowBackground = Color.FromArgb(255, 251, 247, 255);
            this.GroupHeaderBackground = Color.FromArgb(255, 216, 216, 216);
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private Color _headerBackground;
        private Color _rowBackground;
        private Color _groupHeaderBackground;

        public Color GroupHeaderBackground
        {
            get
            {
                return this._groupHeaderBackground;
            }
            set
            {
                if (this._groupHeaderBackground != value)
                {
                    this._groupHeaderBackground = value;
                    OnPropertyChanged("GroupHeaderBackground");
                }
            }
        }
        public Color HeaderBackground
        {
            get
            {
                return this._headerBackground;
            }
            set
            {
                if (this._headerBackground != value)
                {
                    this._headerBackground = value;
                    OnPropertyChanged("HeaderBackground");
                }
            }
        }
        public Color RowBackground
        {
            get
            {
                return this._rowBackground;
            }
            set
            {
                if (this._rowBackground != value)
                {
                    this._rowBackground = value;
                    OnPropertyChanged("RowBackground");
                }
            }
        }


        public bool ExportRadGridViewToPDF(object parameter)
        {
            try
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.DefaultExt = "*.pdf";
                dialog.Filter = "Adobe PDF Document (*.pdf)|*.pdf";

                if (dialog.ShowDialog() == true)
                {
                    RadDocument document = CreateDocument(parameter as RadGridView);

                    document.LayoutMode = DocumentLayoutMode.Paged;
                    document.Measure(RadDocument.MAX_DOCUMENT_SIZE);
                    document.Arrange(new RectangleF(PointF.Empty, document.DesiredSize));
                    document.SectionDefaultPageOrientation = PageOrientation.Landscape;
                    document.SectionDefaultPageMargin = new Telerik.Windows.Documents.Layout.Padding(20);
                    document.SectionDefaultPageSize = new Size(826, 1160);

                    using (Stream output = dialog.OpenFile())
                        new PdfFormatProvider().Export(document, output);

                    return true;
                }

                else
                    return false;
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool ExportRadGridViewToExcel(object parameter)
        {
            try
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.DefaultExt = "*.xls";
                dialog.Filter = "Files(*.xls)|*.xls";

                if (dialog.ShowDialog() == true)
                {
                    RadDocument document = CreateDocument(parameter as RadGridView);

                    document.LayoutMode = DocumentLayoutMode.Paged;
                    document.Measure(RadDocument.MAX_DOCUMENT_SIZE);
                    document.Arrange(new RectangleF(PointF.Empty, document.DesiredSize));
                    document.SectionDefaultPageOrientation = PageOrientation.Landscape;
                    document.SectionDefaultPageMargin = new Telerik.Windows.Documents.Layout.Padding(20);
                    document.SectionDefaultPageSize = new Size(826, 1160);

                    using (Stream output = dialog.OpenFile())
                        new HtmlFormatProvider().Export(document, output);

                    return true;
                }

                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void ExportRadGridViewToPrint(object parameter)
        {
            RadGridView grid = (RadGridView)parameter;
            RadRichTextBox rtb = new RadRichTextBox() { Height = 0 };

            Grid parent = grid.ParentOfType<Grid>();
            if (parent != null && parent.FindName(rtb.Name) == null)
            {
                parent.Children.Add(rtb);
                rtb.ApplyTemplate();
            }

            rtb.Dispatcher.BeginInvoke((Action)(() =>
            {
                rtb.Document = CreateDocument(grid);
            }));

            rtb.Commands.ChangePageOrientationCommand.Execute(PageOrientation.Landscape);
            rtb.ChangeSectionPageOrientation(PageOrientation.Landscape);
            rtb.Document.SectionDefaultPageOrientation = PageOrientation.Landscape;
            Telerik.Windows.Documents.Layout.Padding padding = new Telerik.Windows.Documents.Layout.Padding(20);
            rtb.Commands.ChangePageMarginsCommand.Execute(padding);
            rtb.Document.SectionDefaultPageMargin = padding;
            rtb.ChangeSectionPageMargin(padding);
            rtb.ChangeSectionPageSize(new Size(826, 1160));
            rtb.Document.SectionDefaultPageSize = new Size(826, 1160);
            rtb.ShowComments = false;
            rtb.Commands.ChangeSectionFooterBottomMarginCommand.Execute(0);
            rtb.ChangeSectionFooterBottomMargin(0);
            rtb.Print(Mensagens.tituloMessageBox, Telerik.Windows.Documents.UI.PrintMode.Native);

            rtb.PrintCompleted += (s, e) =>
            {
                parent.Children.Remove(rtb);
            };
        }

        public bool ExportRadChartToPDF(object parameter)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = "*.pdf";
            dialog.Filter = "Adobe PDF Document (*.pdf)|*.pdf";

            if (dialog.ShowDialog() == true)
            {
                RadDocument document = this.CreateDocument(parameter as RadChart);
                document.LayoutMode = DocumentLayoutMode.Paged;
                document.Measure(RadDocument.MAX_DOCUMENT_SIZE);
                document.Arrange(new RectangleF(PointF.Empty, document.DesiredSize));

                PdfFormatProvider provider = new PdfFormatProvider();

                using (Stream output = dialog.OpenFile())
                    provider.Export(document, output);

                return true;
            }

            else
                return false;
        }

        public bool ExportRadChartToExcel(object parameter)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = "*.xls";
            dialog.Filter = "Files(*.xls)|*.xls";

            if (dialog.ShowDialog() == true)
            {
                RadDocument document = this.CreateDocument(parameter as RadChart);
                Stream fileStream = dialog.OpenFile();
                (parameter as RadChart).ExportToExcelML(fileStream);
                fileStream.Close();

                return true;
            }

            else
                return false;
        }

        private RadDocument CreateDocument(RadGridView grid)
        {
            List<GridViewBoundColumnBase> columns = (from c in grid.Columns.OfType<GridViewBoundColumnBase>()
                                                     where c.IsVisible == true
                                                     orderby c.DisplayIndex
                                                     select c).ToList();

            Telerik.Windows.Documents.Model.Table table = new Telerik.Windows.Documents.Model.Table();

            RadDocument document = new RadDocument();

            Telerik.Windows.Documents.Model.Section section = new Telerik.Windows.Documents.Model.Section();
            section.Blocks.Add(table);
            document.Sections.Add(section);

            if (grid.ShowColumnHeaders)
            {
                Telerik.Windows.Documents.Model.TableRow headerRow = new Telerik.Windows.Documents.Model.TableRow();

                if (grid.GroupDescriptors.Count > 0)
                {
                    Telerik.Windows.Documents.Model.TableCell indentCell = new Telerik.Windows.Documents.Model.TableCell();
                    indentCell.PreferredWidth = new TableWidthUnit(grid.GroupDescriptors.Count * 20);
                    indentCell.Background = HeaderBackground;
                    headerRow.Cells.Add(indentCell);
                }

                for (int i = 0; i < columns.Count; i++)
                {
                    Telerik.Windows.Documents.Model.TableCell cell = new Telerik.Windows.Documents.Model.TableCell();
                    cell.Background = HeaderBackground;
                    if (columns[i].Header is string)
                        AddCellValue(cell, columns[i].Header.ToString());
                    else
                        AddCellValue(cell, columns[i].UniqueName);

                    TableWidthUnit t = new TableWidthUnit(TableWidthUnitType.Auto);
                    cell.PreferredWidth = t;
                    headerRow.Cells.Add(cell);
                }

                table.Rows.Add(headerRow);
            }

            if (grid.Items.Groups != null)
            {
                for (int i = 0; i < grid.Items.Groups.Count; i++)
                    AddGroupRow(table, grid.Items.Groups[i] as QueryableCollectionViewGroup, columns, grid);
            }
            else
                AddDataRows(table, grid.Items, columns, grid);


            foreach (GridViewBoundColumnBase b in columns)
            {
                TableWidthUnit unit = new TableWidthUnit(TableWidthUnitType.Auto);
                table.SetGridColumnWidth(b.DisplayIndex, unit);
            }

            return document;
        }

        public RadDocument CreateDocument(RadChart radChart)
        {
            RadDocument document = new RadDocument();

            Section section = new Section();
            Paragraph paragraph = new Paragraph();

            MemoryStream ms = new MemoryStream();
            radChart.ExportToImage(ms, new PngBitmapEncoder());

            double imageWidth = radChart.ActualWidth;
            double imageHeight = radChart.ActualHeight;

            if (imageWidth > 625)
            {
                imageWidth = 625;
                imageHeight = radChart.ActualHeight * imageWidth / radChart.ActualWidth;
            }

            ImageInline image = new ImageInline(ms, new Size(imageWidth, imageHeight), "png");

            paragraph.Inlines.Add(image);
            section.Blocks.Add(paragraph);
            document.Sections.Add(section);

            ms.Close();

            return document;
        }

        private void AddDataRows(Telerik.Windows.Documents.Model.Table table, IList items, IList<GridViewBoundColumnBase> columns, RadGridView grid)
        {
            for (int i = 0; i < items.Count; i++)
            {
                Telerik.Windows.Documents.Model.TableRow row = new Telerik.Windows.Documents.Model.TableRow();

                if (grid.GroupDescriptors.Count > 0)
                {
                    Telerik.Windows.Documents.Model.TableCell indentCell = new Telerik.Windows.Documents.Model.TableCell();
                    indentCell.PreferredWidth = new TableWidthUnit(grid.GroupDescriptors.Count * 20);
                    indentCell.Background = RowBackground;
                    row.Cells.Add(indentCell);
                }

                for (int j = 0; j < columns.Count; j++)
                {
                    object value = columns[j].GetValueForItem(items[i]);
                    if (value is string || value is int || value is double || value is double? || value is int? || value is DateTime)
                    {
                        Telerik.Windows.Documents.Model.TableCell cell = new Telerik.Windows.Documents.Model.TableCell();

                        if (value is DateTime)
                            AddCellValue(cell, value != null ? Convert.ToDateTime(value).ToString("d") : string.Empty);

                        else
                            AddCellValue(cell, value != null ? value.ToString() : string.Empty);

                        cell.PreferredWidth = new TableWidthUnit((float)columns[j].ActualWidth);
                        cell.Background = RowBackground;

                        row.Cells.Add(cell);
                    }
                    else
                    {
                        Telerik.Windows.Documents.Model.TableCell cell = new Telerik.Windows.Documents.Model.TableCell();
                        AddCellValue(cell, value != null ? "/" : string.Empty);
                        cell.PreferredWidth = new TableWidthUnit((float)columns[j].ActualWidth);
                        cell.Background = RowBackground;

                        row.Cells.Add(cell);
                    }
                }

                table.Rows.Add(row);
            }
        }

        private void AddGroupRow(Telerik.Windows.Documents.Model.Table table, QueryableCollectionViewGroup group, IList<GridViewBoundColumnBase> columns, RadGridView grid)
        {
            Telerik.Windows.Documents.Model.TableRow row = new Telerik.Windows.Documents.Model.TableRow();

            int level = GetGroupLevel(group);
            if (level > 0)
            {
                Telerik.Windows.Documents.Model.TableCell cell = new Telerik.Windows.Documents.Model.TableCell();
                cell.PreferredWidth = new TableWidthUnit(level * 20);
                cell.Background = GroupHeaderBackground;
                row.Cells.Add(cell);
            }

            Telerik.Windows.Documents.Model.TableCell aggregatesCell = new Telerik.Windows.Documents.Model.TableCell();
            aggregatesCell.Background = GroupHeaderBackground;
            aggregatesCell.ColumnSpan = columns.Count + (grid.GroupDescriptors.Count > 0 ? 1 : 0) - (level > 0 ? 1 : 0);

            AddCellValue(aggregatesCell, group.Key != null ? group.Key.ToString() : string.Empty);

            foreach (AggregateResult result in group.AggregateResults)
                AddCellValue(aggregatesCell, result.FormattedValue != null ? result.FormattedValue.ToString() : string.Empty);

            row.Cells.Add(aggregatesCell);

            table.Rows.Add(row);

            if (group.HasSubgroups)
            {
                foreach (var g in group.Subgroups)
                    AddGroupRow(table, g as QueryableCollectionViewGroup, columns, grid);
            }
            else
                AddDataRows(table, group.Items, columns, grid);
        }

        private void AddCellValue(Telerik.Windows.Documents.Model.TableCell cell, string value)
        {
            Telerik.Windows.Documents.Model.Paragraph paragraph = new Telerik.Windows.Documents.Model.Paragraph();
            cell.Blocks.Add(paragraph);

            Telerik.Windows.Documents.Model.Span span = new Telerik.Windows.Documents.Model.Span();
            if (value == null || value == string.Empty)
                value = "/";

            span.Text = value;

            paragraph.Inlines.Add(span);
        }

        private int GetGroupLevel(IGroup group)
        {
            int level = 0;

            IGroup parent = group.ParentGroup;

            while (parent != null)
            {
                level++;
                parent = parent.ParentGroup;
            }

            return level;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
