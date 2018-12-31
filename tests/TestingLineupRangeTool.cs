using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LIneupUsageEstimator;
using System.Windows.Forms;
using LineupEngine;

namespace tests
{
    [TestClass]
    public class TestComboBoxPopulation
    {
        private int FULL_CB = 19;

        [TestMethod]
        public void testComboBox_Initalize()
        {
            performFromSelectionTest(-1, FULL_CB);
        }

        [TestMethod]
        public void testComboBox_Set_FROM_Even_when_TO_unselected()
        {
            performFromSelectionTest(9, 10);
        }

        [TestMethod]
        public void testComboBox_Set_FROM_9_when_TO_unselectedL()
        {
            performFromSelectionTest(0, FULL_CB);
        }

        [TestMethod]
        public void testComboBox_Set_FROM_9R_when_TO_unselected()
        {
            performFromSelectionTest(18, 1);
        }

        [TestMethod]
        public void testComboBox_Set_TO_Even_when_BOTH_unselected()
        {
            performToSelectionTest(9, 10);
            performFromSelectionTest(9, 10);
        }

        [TestMethod]
        public void testComboBox_Set_TO_9L_when_FROM_unselected()
        {
            performToSelectionTest(0, 1);
        }

        [TestMethod]
        public void testComboBox_Set_TO_9R_when_FROM_unselected()
        {
            performToSelectionTest(18, FULL_CB);
        }
        [TestMethod]
        public void testComboBox_Set_FROM_9L_when_TO_set_3L()
        {
            performFromSelectionAfterToSelectedTest(0, 6);
        }


        private void performFromSelectionTest(int fromSetting, int extectedToCount)
        {
            LineupMgrDlg dlg = new LineupMgrDlg(true);
            ComboBox cbFROM = new ComboBox();
            ComboBox cbTO = new ComboBox();

            Assert.AreEqual(0, cbFROM.Items.Count);
            Assert.AreEqual(0, cbTO.Items.Count);

            dlg.populateComboBoxes(cbFROM, cbTO, null);

            Assert.AreEqual(19, cbFROM.Items.Count);
            Assert.AreEqual(19, cbTO.Items.Count);

            if (fromSetting != -1) { 
                cbFROM.SelectedIndex = fromSetting;
                dlg.populateComboBoxes(cbFROM, cbTO, cbFROM);
            }
            Assert.AreEqual(FULL_CB, cbFROM.Items.Count);
            Assert.AreEqual(extectedToCount, cbTO.Items.Count);
        }

        private void performToSelectionTest(int toSetting, int extectedFromCount)
        {
            LineupMgrDlg dlg = new LineupMgrDlg(true);
            ComboBox cbFROM = new ComboBox();
            ComboBox cbTO = new ComboBox();

            Assert.AreEqual(0, cbFROM.Items.Count);
            Assert.AreEqual(0, cbTO.Items.Count);

            dlg.populateComboBoxes(cbFROM, cbTO, null);

            Assert.AreEqual(19, cbFROM.Items.Count);
            Assert.AreEqual(19, cbTO.Items.Count);

            if (toSetting != -1)
            {
                cbTO.SelectedIndex = toSetting;
                dlg.populateComboBoxes(cbFROM, cbTO, cbTO);
            }
            Assert.AreEqual(extectedFromCount, cbFROM.Items.Count);
            Assert.AreEqual(FULL_CB, cbTO.Items.Count);
        }

        private void performFromSelectionAfterToSelectedTest(int fromSetting, int toSetting)
        {
            LineupMgrDlg dlg = new LineupMgrDlg(true);
            ComboBox cbFROM = new ComboBox();
            ComboBox cbTO = new ComboBox();

            Assert.AreEqual(0, cbFROM.Items.Count);
            Assert.AreEqual(0, cbTO.Items.Count);

            dlg.populateComboBoxes(cbFROM, cbTO, null);

            Assert.AreEqual(19, cbFROM.Items.Count);
            Assert.AreEqual(19, cbTO.Items.Count);

            // Pre Select TO
            cbTO.SelectedIndex = toSetting;
            dlg.populateComboBoxes(cbFROM, cbTO, cbTO);

            // User selects FROM
            cbFROM.SelectedIndex = fromSetting;
            dlg.populateComboBoxes(cbFROM, cbTO, cbFROM);
            
            int from = cbFROM.Items.Count;
            int to = cbTO.Items.Count;

            String sFrom = ((LineupBalanceItem)cbFROM.SelectedItem).ToString();
            String sTo = ((LineupBalanceItem)cbTO.SelectedItem).ToString();
            return;
        }
    }
}
