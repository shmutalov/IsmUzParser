namespace IsmUzParser
{
    partial class MainWindow
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.ParseButton = new System.Windows.Forms.Button();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.StatusProgress = new System.Windows.Forms.ProgressBar();
            this.MessageList = new System.Windows.Forms.ListBox();
            this.RefreshNamesTableButton = new System.Windows.Forms.Button();
            this.FilterText = new System.Windows.Forms.TextBox();
            this.FindButton = new System.Windows.Forms.Button();
            this.NamesTable = new System.Windows.Forms.ListView();
            this.NamesTableLetterColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.NamesTableGenderColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.NamesTableNameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.NamesTableMeaningColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // ParseButton
            // 
            this.ParseButton.Location = new System.Drawing.Point(12, 12);
            this.ParseButton.Name = "ParseButton";
            this.ParseButton.Size = new System.Drawing.Size(75, 23);
            this.ParseButton.TabIndex = 0;
            this.ParseButton.Text = "Parse";
            this.ParseButton.UseVisualStyleBackColor = true;
            // 
            // StatusLabel
            // 
            this.StatusLabel.AutoSize = true;
            this.StatusLabel.Location = new System.Drawing.Point(12, 38);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(40, 13);
            this.StatusLabel.TabIndex = 1;
            this.StatusLabel.Text = "Status:";
            // 
            // StatusProgress
            // 
            this.StatusProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StatusProgress.Location = new System.Drawing.Point(12, 54);
            this.StatusProgress.Name = "StatusProgress";
            this.StatusProgress.Size = new System.Drawing.Size(823, 23);
            this.StatusProgress.TabIndex = 2;
            // 
            // MessageList
            // 
            this.MessageList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MessageList.FormattingEnabled = true;
            this.MessageList.HorizontalScrollbar = true;
            this.MessageList.Location = new System.Drawing.Point(12, 83);
            this.MessageList.Name = "MessageList";
            this.MessageList.ScrollAlwaysVisible = true;
            this.MessageList.Size = new System.Drawing.Size(823, 173);
            this.MessageList.TabIndex = 3;
            // 
            // RefreshNamesTableButton
            // 
            this.RefreshNamesTableButton.Location = new System.Drawing.Point(12, 262);
            this.RefreshNamesTableButton.Name = "RefreshNamesTableButton";
            this.RefreshNamesTableButton.Size = new System.Drawing.Size(75, 23);
            this.RefreshNamesTableButton.TabIndex = 4;
            this.RefreshNamesTableButton.Text = "Refresh";
            this.RefreshNamesTableButton.UseVisualStyleBackColor = true;
            // 
            // FilterText
            // 
            this.FilterText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterText.Location = new System.Drawing.Point(93, 264);
            this.FilterText.Name = "FilterText";
            this.FilterText.Size = new System.Drawing.Size(661, 20);
            this.FilterText.TabIndex = 5;
            // 
            // FindButton
            // 
            this.FindButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FindButton.Location = new System.Drawing.Point(760, 264);
            this.FindButton.Name = "FindButton";
            this.FindButton.Size = new System.Drawing.Size(75, 23);
            this.FindButton.TabIndex = 6;
            this.FindButton.Text = "Find";
            this.FindButton.UseVisualStyleBackColor = true;
            // 
            // NamesTable
            // 
            this.NamesTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NamesTable.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NamesTableLetterColumn,
            this.NamesTableGenderColumn,
            this.NamesTableNameColumn,
            this.NamesTableMeaningColumn});
            this.NamesTable.GridLines = true;
            this.NamesTable.Location = new System.Drawing.Point(12, 291);
            this.NamesTable.MultiSelect = false;
            this.NamesTable.Name = "NamesTable";
            this.NamesTable.Size = new System.Drawing.Size(823, 182);
            this.NamesTable.TabIndex = 7;
            this.NamesTable.UseCompatibleStateImageBehavior = false;
            this.NamesTable.View = System.Windows.Forms.View.Details;
            // 
            // NamesTableLetterColumn
            // 
            this.NamesTableLetterColumn.Text = "Letter";
            // 
            // NamesTableGenderColumn
            // 
            this.NamesTableGenderColumn.Text = "Gender";
            // 
            // NamesTableNameColumn
            // 
            this.NamesTableNameColumn.Text = "Name";
            this.NamesTableNameColumn.Width = 300;
            // 
            // NamesTableMeaningColumn
            // 
            this.NamesTableMeaningColumn.Text = "Meaning";
            this.NamesTableMeaningColumn.Width = 400;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(847, 485);
            this.Controls.Add(this.NamesTable);
            this.Controls.Add(this.FindButton);
            this.Controls.Add(this.FilterText);
            this.Controls.Add(this.RefreshNamesTableButton);
            this.Controls.Add(this.MessageList);
            this.Controls.Add(this.StatusProgress);
            this.Controls.Add(this.StatusLabel);
            this.Controls.Add(this.ParseButton);
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ism.Uz Parser";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ParseButton;
        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.ProgressBar StatusProgress;
        private System.Windows.Forms.ListBox MessageList;
        private System.Windows.Forms.Button RefreshNamesTableButton;
        private System.Windows.Forms.TextBox FilterText;
        private System.Windows.Forms.Button FindButton;
        private System.Windows.Forms.ListView NamesTable;
        private System.Windows.Forms.ColumnHeader NamesTableLetterColumn;
        private System.Windows.Forms.ColumnHeader NamesTableGenderColumn;
        private System.Windows.Forms.ColumnHeader NamesTableNameColumn;
        private System.Windows.Forms.ColumnHeader NamesTableMeaningColumn;
    }
}

