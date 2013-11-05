namespace Kaggle
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.bLoadData = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.bTrain = new System.Windows.Forms.Button();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.bStop = new System.Windows.Forms.Button();
            this.bSave = new System.Windows.Forms.Button();
            this.bLoad = new System.Windows.Forms.Button();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.bTest = new System.Windows.Forms.Button();
            this.bExport = new System.Windows.Forms.Button();
            this.zedGraphControl1 = new ZedGraph.ZedGraphControl();
            this.bTestTest = new System.Windows.Forms.Button();
            this.bTestResult = new System.Windows.Forms.Button();
            this.bOpenData = new System.Windows.Forms.Button();
            this.bSaveData = new System.Windows.Forms.Button();
            this.bTrainCascades = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.SuspendLayout();
            // 
            // bLoadData
            // 
            this.bLoadData.Location = new System.Drawing.Point(12, 12);
            this.bLoadData.Name = "bLoadData";
            this.bLoadData.Size = new System.Drawing.Size(75, 23);
            this.bLoadData.TabIndex = 0;
            this.bLoadData.Text = "Load Data";
            this.bLoadData.UseVisualStyleBackColor = true;
            this.bLoadData.Click += new System.EventHandler(this.bLoadData_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 74);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(256, 256);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(274, 74);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(256, 256);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 2;
            this.pictureBox2.TabStop = false;
            // 
            // bTrain
            // 
            this.bTrain.Location = new System.Drawing.Point(174, 12);
            this.bTrain.Name = "bTrain";
            this.bTrain.Size = new System.Drawing.Size(75, 23);
            this.bTrain.TabIndex = 3;
            this.bTrain.Text = "Train";
            this.bTrain.UseVisualStyleBackColor = true;
            this.bTrain.Click += new System.EventHandler(this.bTrain_Click);
            // 
            // pictureBox3
            // 
            this.pictureBox3.Location = new System.Drawing.Point(274, 336);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(256, 256);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 4;
            this.pictureBox3.TabStop = false;
            // 
            // bStop
            // 
            this.bStop.Location = new System.Drawing.Point(174, 41);
            this.bStop.Name = "bStop";
            this.bStop.Size = new System.Drawing.Size(75, 23);
            this.bStop.TabIndex = 5;
            this.bStop.Text = "Stop";
            this.bStop.UseVisualStyleBackColor = true;
            this.bStop.Click += new System.EventHandler(this.bStop_Click);
            // 
            // bSave
            // 
            this.bSave.Location = new System.Drawing.Point(335, 12);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(75, 23);
            this.bSave.TabIndex = 6;
            this.bSave.Text = "Save";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // bLoad
            // 
            this.bLoad.Location = new System.Drawing.Point(416, 12);
            this.bLoad.Name = "bLoad";
            this.bLoad.Size = new System.Drawing.Size(75, 23);
            this.bLoad.TabIndex = 7;
            this.bLoad.Text = "Load";
            this.bLoad.UseVisualStyleBackColor = true;
            this.bLoad.Click += new System.EventHandler(this.bLoad_Click);
            // 
            // pictureBox4
            // 
            this.pictureBox4.Location = new System.Drawing.Point(12, 336);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(256, 256);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox4.TabIndex = 8;
            this.pictureBox4.TabStop = false;
            // 
            // bTest
            // 
            this.bTest.Location = new System.Drawing.Point(335, 41);
            this.bTest.Name = "bTest";
            this.bTest.Size = new System.Drawing.Size(75, 23);
            this.bTest.TabIndex = 9;
            this.bTest.Text = "Test Train";
            this.bTest.UseVisualStyleBackColor = true;
            this.bTest.Click += new System.EventHandler(this.bTest_Click);
            // 
            // bExport
            // 
            this.bExport.Location = new System.Drawing.Point(497, 12);
            this.bExport.Name = "bExport";
            this.bExport.Size = new System.Drawing.Size(75, 23);
            this.bExport.TabIndex = 10;
            this.bExport.Text = "Export";
            this.bExport.UseVisualStyleBackColor = true;
            this.bExport.Click += new System.EventHandler(this.bExport_Click);
            // 
            // zedGraphControl1
            // 
            this.zedGraphControl1.Location = new System.Drawing.Point(536, 74);
            this.zedGraphControl1.Name = "zedGraphControl1";
            this.zedGraphControl1.ScrollGrace = 0D;
            this.zedGraphControl1.ScrollMaxX = 0D;
            this.zedGraphControl1.ScrollMaxY = 0D;
            this.zedGraphControl1.ScrollMaxY2 = 0D;
            this.zedGraphControl1.ScrollMinX = 0D;
            this.zedGraphControl1.ScrollMinY = 0D;
            this.zedGraphControl1.ScrollMinY2 = 0D;
            this.zedGraphControl1.Size = new System.Drawing.Size(676, 518);
            this.zedGraphControl1.TabIndex = 11;
            // 
            // bTestTest
            // 
            this.bTestTest.Location = new System.Drawing.Point(416, 41);
            this.bTestTest.Name = "bTestTest";
            this.bTestTest.Size = new System.Drawing.Size(75, 23);
            this.bTestTest.TabIndex = 12;
            this.bTestTest.Text = "Test Test";
            this.bTestTest.UseVisualStyleBackColor = true;
            this.bTestTest.Click += new System.EventHandler(this.bTestTest_Click);
            // 
            // bTestResult
            // 
            this.bTestResult.Location = new System.Drawing.Point(497, 41);
            this.bTestResult.Name = "bTestResult";
            this.bTestResult.Size = new System.Drawing.Size(75, 23);
            this.bTestResult.TabIndex = 13;
            this.bTestResult.Text = "Test Result";
            this.bTestResult.UseVisualStyleBackColor = true;
            this.bTestResult.Click += new System.EventHandler(this.bTestResult_Click);
            // 
            // bOpenData
            // 
            this.bOpenData.Location = new System.Drawing.Point(93, 12);
            this.bOpenData.Name = "bOpenData";
            this.bOpenData.Size = new System.Drawing.Size(75, 23);
            this.bOpenData.TabIndex = 14;
            this.bOpenData.Text = "Open Data";
            this.bOpenData.UseVisualStyleBackColor = true;
            this.bOpenData.Click += new System.EventHandler(this.bOpenData_Click);
            // 
            // bSaveData
            // 
            this.bSaveData.Location = new System.Drawing.Point(93, 41);
            this.bSaveData.Name = "bSaveData";
            this.bSaveData.Size = new System.Drawing.Size(75, 23);
            this.bSaveData.TabIndex = 15;
            this.bSaveData.Text = "Save Data";
            this.bSaveData.UseVisualStyleBackColor = true;
            this.bSaveData.Click += new System.EventHandler(this.bSaveData_Click);
            // 
            // bTrainCascades
            // 
            this.bTrainCascades.Location = new System.Drawing.Point(710, 12);
            this.bTrainCascades.Name = "bTrainCascades";
            this.bTrainCascades.Size = new System.Drawing.Size(75, 52);
            this.bTrainCascades.TabIndex = 16;
            this.bTrainCascades.Text = "Train Cascades";
            this.bTrainCascades.UseVisualStyleBackColor = true;
            this.bTrainCascades.Click += new System.EventHandler(this.bTrainCascades_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1224, 612);
            this.Controls.Add(this.bTrainCascades);
            this.Controls.Add(this.bSaveData);
            this.Controls.Add(this.bOpenData);
            this.Controls.Add(this.bTestResult);
            this.Controls.Add(this.bTestTest);
            this.Controls.Add(this.zedGraphControl1);
            this.Controls.Add(this.bExport);
            this.Controls.Add(this.bTest);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.bLoad);
            this.Controls.Add(this.bSave);
            this.Controls.Add(this.bStop);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.bTrain);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.bLoadData);
            this.Name = "MainForm";
            this.Text = "PragmaLearn";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bLoadData;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button bTrain;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Button bStop;
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.Button bLoad;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.Button bTest;
        private System.Windows.Forms.Button bExport;
        private ZedGraph.ZedGraphControl zedGraphControl1;
        private System.Windows.Forms.Button bTestTest;
        private System.Windows.Forms.Button bTestResult;
        private System.Windows.Forms.Button bOpenData;
        private System.Windows.Forms.Button bSaveData;
        private System.Windows.Forms.Button bTrainCascades;
    }
}

