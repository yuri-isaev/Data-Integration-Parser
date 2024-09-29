using System.ComponentModel;

namespace CardsDataIntegration;

public partial class MainForm : Form
{
    private IContainer components = null;
    private DataGridView dataGridView;
    private Button btnImport;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.dataGridView = new DataGridView();
        this.btnImport = new Button();
        ((ISupportInitialize) (this.dataGridView)).BeginInit();
        this.SuspendLayout();

        // dataGridView
        this.dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.dataGridView.Location = new Point(12, 12);
        this.dataGridView.Name = "dataGridView";
        this.dataGridView.Size = new Size(760, 400);
        this.dataGridView.TabIndex = 0;
        this.dataGridView.CellValueChanged += new DataGridViewCellEventHandler(this.dataGridView_CellValueChanged);

        // btnImport
        this.btnImport.Location = new Point(12, 418);
        this.btnImport.Name = "btnImport";
        this.btnImport.Size = new Size(75, 23);
        this.btnImport.TabIndex = 1;
        this.btnImport.Text = "Import";
        this.btnImport.UseVisualStyleBackColor = true;
        this.btnImport.Click += new EventHandler(this.btnImport_Click);

        // MainForm
        this.ClientSize = new Size(784, 461);
        this.Controls.Add(this.btnImport);
        this.Controls.Add(this.dataGridView);
        this.Name = "MainForm";
        this.Text = "Управление клиентами";
        ((ISupportInitialize) (this.dataGridView)).EndInit();
        this.ResumeLayout(false);
    }
}