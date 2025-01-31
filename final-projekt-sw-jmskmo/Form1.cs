//#define RUNTIME // comment for DEBUG MODE

using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace final_projekt_sw_jmskmo {
    public partial class Form1 : Form {
        bool allow_pointer_lines = false;
        bool custom_image_selected = false;
        bool updateColorInt = false;
        int elements = 0;
        byte tone = 0;
        int a = 0, s = 0, aa = 0, fe = 0;
        int dlugosc = 0, dlugosc_dyskr = 0;
        double gupSize = 0.0;
        Image<Bgr, byte> image1, linedImage;
        Image<Bgr, byte> originalImage;
        OpenFileDialog fileDialog = new OpenFileDialog();
        List<int> elementData_min_x = new List<int>();
        List<int> elementData_min_y = new List<int>();
        List<int> elementData_max_x = new List<int>();
        List<int> elementData_max_y = new List<int>();

        List<int> elementData_max_x_dist = new List<int>();
        List<int> elementData_max_y_dist = new List<int>();

        List<double> element_size = new List<double>();
        List<double> element_empty = new List<double>();

        List<int> element_naibour = new List<int>();

        //public struct elementNaibour {
        //    public elementNaibour(List<int> elementID, List<int> naibourID)
        //    {
        //        element = elementID;
        //        naibour = naibourID;
        //    }    

        //    public List<int> element { get; }
        //    public List<int> naibour { get; }
        //}

        public Form1() {
            InitializeComponent();
            image1 = new Image<Bgr, byte>(800, 600);
            linedImage = new Image<Bgr, byte>(800, 600);
        }
        private void button_load_image_Click(object sender, EventArgs e) {
            try {
                fileDialog = new OpenFileDialog();
                fileDialog.Filter = "Image Files (*.jpg; *.png; *.bmp;)|*.jpg; *.png; *.bmp;";
                if (fileDialog.ShowDialog() == DialogResult.OK) {
                    originalImage = new Image<Bgr, byte>(fileDialog.FileName);
                    pictureBox_main.Image = originalImage.ToBitmap();
                    image1 = originalImage;
                }
                custom_image_selected = true;
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
                custom_image_selected = false;
                image1 = null;
            }
        }

        private void button_set_block_color_Click(object sender, EventArgs e) {
            if (custom_image_selected) {
                Cursor.Position = PointToScreen(new Point(400, 350));
                allow_pointer_lines = true;
                updateColorInt = true;
            } else {
                System.Windows.Forms.MessageBox.Show("Import your image first");
            }
        }

        private void button_clear_image_Click(object sender, EventArgs e) {
            custom_image_selected = false;
            image1 = null;
            pictureBox_main.Image = null;
        }

        private void pictureBox_main_MouseClick(object sender, MouseEventArgs e) {
            allow_pointer_lines = false;
            updateColorInt = false;
        }

        private void updateColorNumericalInputs(int x, int y) {
            byte[,,] temp;
            temp = image1.Data;

            numericUpDown_b.Value = temp[y, x, 0];
            numericUpDown_g.Value = temp[y, x, 1];
            numericUpDown_r.Value = temp[y, x, 2];
        }
        private bool ifFoundSelectedPixelColor(byte[,,] tempImage, int x, int y) {
            if (tempImage[y, x, 0] == numericUpDown_b.Value &&
                tempImage[y, x, 1] == numericUpDown_g.Value &&
                tempImage[y, x, 2] == numericUpDown_r.Value
                ) { return true; } else { return false; }
        }

        private bool ifFoundSelectedPixelColor(byte[,,] tempImage, int x, int y, byte tone) {
            if (tempImage[y, x, 0] == tone &&
                tempImage[y, x, 1] == tone &&
                tempImage[y, x, 2] == tone
                ) { return true; } else { return false; }
        }

        private void fillPixel(byte[,,] tempImage, int x, int y, byte tone) {
            tempImage[y, x, 0] = tone;
            tempImage[y, x, 1] = tone;
            tempImage[y, x, 2] = tone;
        }

        private void markElementAsUnique(byte[,,] tempImage, int x, int y, byte tone) {
            fillPixel(tempImage, x, y, tone);

            for (int yy = y; yy < image1.Height - 1; yy++) {
                for (int xx = x; xx < image1.Width - 1; xx++) {
                    if (ifFoundSelectedPixelColor(tempImage, xx, yy, tone)) {
                        if (ifFoundSelectedPixelColor(tempImage, xx + 1, yy)) {
                            fillPixel(tempImage, xx + 1, yy, tone);
                        }
                        if (ifFoundSelectedPixelColor(tempImage, xx, yy + 1)) {
                            fillPixel(tempImage, xx, yy + 1, tone);
                        }
                    }
                }
            }

            for (int yy = y; yy < image1.Height - 1; yy++) {
                for (int xx = x; xx > 1; xx--) {
                    if (ifFoundSelectedPixelColor(tempImage, xx, yy, tone)) {
                        if (ifFoundSelectedPixelColor(tempImage, xx - 1, yy)) {
                            fillPixel(tempImage, xx - 1, yy, tone);
                        }
                        if (ifFoundSelectedPixelColor(tempImage, xx, yy + 1)) {
                            fillPixel(tempImage, xx, yy + 1, tone);
                        }
                    }
                }
            }

            for (int yy = y; yy > 1; yy--) {
                for (int xx = x; xx < image1.Width - 1; xx++) {
                    if (ifFoundSelectedPixelColor(tempImage, xx, yy, tone)) {
                        if (ifFoundSelectedPixelColor(tempImage, xx + 1, yy)) {
                            fillPixel(tempImage, xx + 1, yy, tone);
                        }
                        if (ifFoundSelectedPixelColor(tempImage, xx, yy - 1)) {
                            fillPixel(tempImage, xx, yy - 1, tone);
                        }
                    }
                }
            }

            for (int yy = y; yy > 1; yy--) {
                for (int xx = x; xx > 1; xx--) {
                    if (ifFoundSelectedPixelColor(tempImage, xx, yy, tone)) {
                        if (ifFoundSelectedPixelColor(tempImage, xx - 1, yy)) {
                            fillPixel(tempImage, xx - 1, yy, tone);
                        }
                        if (ifFoundSelectedPixelColor(tempImage, xx, yy - 1)) {
                            fillPixel(tempImage, xx, yy - 1, tone);
                        }
                    }
                }
            }
        }

        private void button_detect_Click(object sender, EventArgs e) {
            byte[,,] temp;
            temp = image1.Data;
            tone = 0;
            elements = 0;

            for (int y = 1; y < image1.Height - 1; y++) {
                for (int x = 1; x < image1.Width - 1; x++) {
                    if (ifFoundSelectedPixelColor(temp, x, y)) {
                        markElementAsUnique(temp, x, y, tone);
                        tone += 1;
                        elements++;
                    }
                }
            }

            image1.Data = temp;
            pictureBox_main.Image = image1.Bitmap;
        }

        int[] w_number = { 0, 0, 0, 0, 0, 0, 0 }; //!Max layer count: 7
        private void showData() {
            richTextBox_data.Clear();
            richTextBox_data.Text = string.Empty;
            elementData_max_y_dist = elementData_max_y.Distinct().ToList();

            int minW = elementData_max_y_dist.Max();

            richTextBox_data.Text = String.Format("Layers: {0} \nLowestLayerY: {1}", elementData_max_y_dist.Count, minW);

            for (int layer = 0; layer < elementData_max_y_dist.Count; layer++) {
                for (int i = 0; i < elements; i++) {
                    if (elementData_max_y_dist[layer] == elementData_max_y[i]) {
                        w_number[layer]++;
                    }
                }

                CvInvoke.PutText(
                    image1,
                    String.Format("W:{0}", layer),
                    new Point(0, elementData_max_y_dist[layer] - 5),
                    Emgu.CV.CvEnum.FontFace.HersheyDuplex,
                    1,
                    new MCvScalar(0, 255, 255),
                    1
                );

                CvInvoke.Line(
                    image1,
                    new Point(0, elementData_max_y_dist[layer] + 1),
                    new Point(pictureBox_main.Width, elementData_max_y_dist[layer] + 1),
                    new MCvScalar(0, 255, 255),
                    1
                );
            }

            for (int i = 0; i <= elementData_max_y_dist.Count - 1; i++) {
                richTextBox_usr.AppendText(String.Format("W{0} elem: {1} pcs\n", i, w_number[i]));
            }
        }

        private int firstElementOfLayer(int layer) {
            int elemCount = 0;
            for (int i = 0; i < layer; i++) {
                elemCount += w_number[i];
            }
            return elemCount;
        }

        private int descretize(int dlugosc) {
            switch (dlugosc) {
                case 147: return 1;
                case 197: return 2;
                case 247: return 3;
                default: return 0;
            }
        }

        private void showInfo() {
            richTextBox_info.Clear();
            richTextBox_info.Text = string.Empty;

            richTextBox_main.Clear();
            richTextBox_main.Text = string.Empty;

            richTextBox_sasiady.Clear();
            richTextBox_sasiady.Text = string.Empty;

            byte[,,] temp_data;
            temp_data = image1.Data;

            for (int i = 0; i < elements; i++) { //for every element
                element_size.Add(Math.Sqrt(
                    Math.Pow((elementData_max_x[i] - elementData_min_x[i]), 2) +
                    Math.Pow((elementData_max_y[i] - elementData_min_y[i]), 2)
                ));
                richTextBox_info.AppendText(String.Format("Size of {0} elem: {1}\n", i, (int)element_size[i]));
            }

            for (int layer = 0; layer < elementData_max_y_dist.Count; layer++) {
                if (w_number[layer] > 1) { //layer with 2 or more elements
                    a = firstElementOfLayer(layer);
                    for (int i = 0; i < w_number[layer] - 1; i++) {
                        gupSize = Math.Sqrt(Math.Pow((elementData_max_x[a + i] - elementData_min_x[a + i + 1]), 2) + Math.Pow((elementData_max_y[a + i] - elementData_min_y[a + i + 1]), 2));
#if !RUNTIME
                        if (gupSize >= 80) {
                            CvInvoke.PutText(
                                image1,
                                String.Format("g:{0}", (int)gupSize),
                                new Point(elementData_max_x[a + i], elementData_min_y[a + i] + (elementData_max_y[a + i] - elementData_min_y[a + i + 1]) / 2),
                                Emgu.CV.CvEnum.FontFace.HersheyDuplex,
                                1,
                                new MCvScalar(0, 100, 255),
                                2
                            );
                            CvInvoke.Line(
                                image1,
                                new Point(elementData_max_x[a + i], elementData_max_y[a + i]),
                                new Point(elementData_min_x[a + i + 1], elementData_min_y[a + i + 1]),
                                new MCvScalar(0, 100, 255),
                                1
                            );
                        }
#endif
                    }
                    richTextBox_info.AppendText("\n");
                }

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //                                              ELEMENTY I POWIETRZE                                           //
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                richTextBox_main.AppendText(String.Format("W{0}: (", layer));
                s = firstElementOfLayer(layer);
                for (int i = 0; i < w_number[layer] - 1; i++) {
                    //--------------------- K bez ostatniego
                    richTextBox_main.AppendText("K");
                    dlugosc = elementData_max_x[s + i] - elementData_min_x[s + i];
                    richTextBox_main.AppendText(String.Format("{0},", descretize(dlugosc)));
                    //--------------------- P
                    gupSize = Math.Sqrt(Math.Pow((elementData_max_x[s + i] - elementData_min_x[s + i + 1]), 2) + Math.Pow((elementData_max_y[s + i] - elementData_min_y[s + i + 1]), 2));
                    if (gupSize >= 100) {
                        richTextBox_main.AppendText("P,");
                    }
                }
                //--------------------- K ostatni
                richTextBox_main.AppendText("K");
                dlugosc = elementData_max_x[s + w_number[layer] - 1] - elementData_min_x[s + w_number[layer] - 1];
                richTextBox_main.AppendText(String.Format("{0},", descretize(dlugosc)));

                richTextBox_main.Text = richTextBox_main.Text.Substring(0, richTextBox_main.Text.Length - 1);
                richTextBox_main.AppendText(")");
                richTextBox_main.AppendText("\n");
            }

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //                                              SASIADSTWO ELEMENTOW                                           //
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //Image<Bgr, byte> swap = new Image<Bgr, byte>(800, 600);
            //swap = image1;
            //temp_data = swap.Data;
            //List<int> element_temp = new List<int>();
            List<int> naibour_temp = new List<int>();
            //elementNaibour en = new elementNaibour();



            // program zadziala tylko jesli masz 32 Gb RAM
            List<int>[] dataset = { new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), };

            //for (int layer = 0; layer < 2; layer++) {
            //    dataset[layer].Add('1');
            //}

            for (int layer = 0; layer < elementData_max_y_dist.Count; layer++) {
                fe = firstElementOfLayer(layer);
                for (int i = 0; i < w_number[layer]; i++) { // dla kazdego elementu z danej warstwy
                    for (int line = 0; line < 10; line++) { // max 11 lines per element
                        if (elementData_min_x[fe + i] + line * 30 < elementData_max_x[fe + i]) { // jezeli nie wychodzi poza zakres
#if !RUNTIME
                            CvInvoke.Line(
                                image1,
                                new Point(elementData_min_x[fe + i] + line * 30, elementData_max_y[fe + i]),
                                new Point(elementData_min_x[fe + i] + line * 30, elementData_max_y[fe + i] + 20),
                                new MCvScalar(120, 80, 210),
                                1
                            );
#endif
                            //richTextBox_sasiady.AppendText(String.Format("_{0}-", temp_data[elementData_max_y[fe + i],
                            //                                                                elementData_min_x[fe + i] + line * 30 + 1,
                            //                                                                0]));
                            dataset[i].Add(elementData_min_x[fe + i] + line * 30 + 1);
                        }
                    }
#if !RUNTIME
                    CvInvoke.Line(
                        image1,
                        new Point(elementData_max_x[fe + i], elementData_max_y[fe + i]),
                        new Point(elementData_max_x[fe + i], elementData_max_y[fe + i] + 20),
                        new MCvScalar(120, 80, 210),
                        1
                    );
#endif
                }

                if (layer != elementData_max_y_dist.Count - 1) { // zawsze oproc ostatniej warstwy
                    richTextBox_sasiady.AppendText(String.Format("S{0}{1}: ", layer, layer + 1));




                    richTextBox_sasiady.AppendText("\n");
                }
            }
            richTextBox_sasiady.Text = richTextBox_sasiady.Text.Substring(0, richTextBox_sasiady.Text.Length - 1); //deleate last '\n'
        }

        private void button_detectCorners_Click(object sender, EventArgs e) {
            byte[,,] temp;
            temp = image1.Data;
            int min_x = image1.Width;
            int min_y = image1.Height;
            int max_x = 0, max_y = 0;

            for (int elem = 0; elem < elements; elem++) {
                min_x = image1.Width;
                min_y = image1.Height;
                max_x = 0;
                max_y = 0;

                for (int x = 0; x < image1.Width; x++) {
                    for (int y = 0; y < image1.Height; y++) {

                        if (ifFoundSelectedPixelColor(temp, x, y, (byte)(elem * 1))) {
                            if (min_x > x) { min_x = x; }
                            if (min_y > y) { min_y = y; }
                            if (max_x < x) { max_x = x; }
                            if (max_y < y) { max_y = y; }
                        }
                    }
                }
                elementData_min_x.Add(min_x);
                elementData_min_y.Add(min_y);
                elementData_max_x.Add(max_x);
                elementData_max_y.Add(max_y);
            }

            for (int i = 0; i < elements; i++) {
#if !RUNTIME
                CvInvoke.Circle(image1, new Point(elementData_min_x[i], elementData_min_y[i]), 5, new MCvScalar(255, 255, 0));
                CvInvoke.Circle(image1, new Point(elementData_max_x[i], elementData_max_y[i]), 5, new MCvScalar(255, 255, 0));

                //Printing element ID on each elements
                CvInvoke.PutText(
                    image1,
                    String.Format("E:{0}", i),
                    new Point((elementData_min_x[i] + elementData_max_x[i]) / 2, (elementData_min_y[i] + elementData_max_y[i]) / 2),
                    Emgu.CV.CvEnum.FontFace.HersheyDuplex,
                    1,
                    new MCvScalar(0, 255, 255),
                    1
                );
#endif
            }

            //Wypellnienie textboksow
            showData();

            showInfo();

            pictureBox_main.Image = image1.Bitmap;
        }

        private void pictureBox_main_MouseMove(object sender, MouseEventArgs e) {
            if (custom_image_selected && allow_pointer_lines && updateColorInt) {
                updateColorNumericalInputs(e.X, e.Y);

                label_position.Text = String.Format("X: {0}, Y: {1}", e.X, e.Y);
                label_position.Location = new Point(e.X + 10, e.Y + 20);
                label_position.Visible = true;
                label_position.Parent = pictureBox_main;
                label_position.BackColor = System.Drawing.Color.Transparent;
            } else {
                label_position.Visible = false;
            }
        }
    }
}
