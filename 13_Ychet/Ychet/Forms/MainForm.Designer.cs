namespace Ychet
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
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
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            
            this.transactionsListView = new System.Windows.Forms.ListView();
            this.accountsListBox = new System.Windows.Forms.ListBox();
            this.lblTotalBalance = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // transactionsListView
            // 
            this.transactionsListView.HideSelection = false;
            this.transactionsListView.Location = new System.Drawing.Point(0, 0);
            this.transactionsListView.Name = "transactionsListView";
            this.transactionsListView.Size = new System.Drawing.Size(121, 97);
            this.transactionsListView.TabIndex = 0;
            this.transactionsListView.UseCompatibleStateImageBehavior = false;
            this.transactionsListView.View = System.Windows.Forms.View.Details;
            // 
            // accountsListBox
            // 
            this.accountsListBox.Location = new System.Drawing.Point(0, 0);
            this.accountsListBox.Name = "accountsListBox";
            this.accountsListBox.Size = new System.Drawing.Size(120, 96);
            this.accountsListBox.TabIndex = 0;
            // 
            // lblTotalBalance
            // 
            this.lblTotalBalance.Location = new System.Drawing.Point(0, 0);
            this.lblTotalBalance.Name = "lblTotalBalance";
            this.lblTotalBalance.Size = new System.Drawing.Size(100, 23);
            this.lblTotalBalance.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        #endregion
    }
}

