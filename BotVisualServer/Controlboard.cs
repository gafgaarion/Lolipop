using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using CommunicationHandler;
using CommunicationHandler.Impl;
using System.Reflection;
using FFXIWebObserver;
using FFXICommands.Commands;
using FFXIEvents.Events;
using FFXIWorldKnowledge;
using FFXIWorldKnowledge.Impl;
using FFXIMemoryObserver;
using FFXIMemoryObserver.Impl;
using Commons;
using FFACETools;
using BotServer;
using ZedGraph;
using FFXICommands;
using FFXIStrategies;
using FFXIStrategies.Impl;
using FFXIAggregateRoots;
using System.Threading;

namespace BotVisualServer
{
    public partial class Controlboard : Form
    {
        private Server instance;
        private IObserver observer = null;
        private IGateway gateway;

        private Zone mapLoaded = Zone.Unknown;
        private GraphPane myPane;
        private IMemoryInspector memoryInspector;
        private IStrategy strategy = null;
        private D3DMap mapLib = null;
        private frmLoading loader = new frmLoading();

        public Controlboard()
        {
            InitializeComponent();


            GlobalDelegates.onAvailable = new GlobalDelegates.onCharacterAvailableForAction(this.UpdateAvailable);
            GlobalDelegates.onHasBeenInitialized = new GlobalDelegates.onCharacterHasBeenInitialized(this.UpdateHasBeenInitialized);
            GlobalDelegates.onHasDisconnected = new GlobalDelegates.onCharacterHasDisconnected(this.UpdateDisconnected);
            GlobalDelegates.onHasMoved = new GlobalDelegates.onCharacterHasMoved(this.UpdateMoved);
            GlobalDelegates.onHPHasChanged = new GlobalDelegates.onCharacterHPHasChanged(this.UpdateHPChanged);
            GlobalDelegates.onIsBusyWithAction = new GlobalDelegates.onCharacterIsBusyWithAction(this.UpdateBusyWithAction);
            GlobalDelegates.onMPHasChanged = new GlobalDelegates.onCharacterMPHasChanged(this.UpdateMPChanged);
            GlobalDelegates.onTPHasChanged = new GlobalDelegates.onCharacterTPHasChanged(this.UpdateTPChanged);
            GlobalDelegates.onMapHasChanged = new GlobalDelegates.onCharacterMapHasChanged(this.UpdateMap);
            GlobalDelegates.onChangedTarget = new GlobalDelegates.onCharacterHasChangedTarget(this.UpdateChangedTarget);
            GlobalDelegates.onCastProgressChanged = new GlobalDelegates.onCharacterCastProgressChanged(this.UpdateCastingProgress);
            GlobalDelegates.onLoginStatusChanged = new GlobalDelegates.onCharacterLoginStatusChanged(this.UpdateLoginStatus);
            GlobalDelegates.onNameChanged = new GlobalDelegates.onCharacterNameChanged(this.UpdateNameChanged);
            GlobalDelegates.onViewModeChanged = new GlobalDelegates.onCharacterViewModeChanged(this.UpdateViewModeChanged);
            GlobalDelegates.onStatusHasChanged = new GlobalDelegates.onCharacterStatusHasChanged(this.UpdateStatusChanged);
            GlobalDelegates.onStatusEffectsHaveChanged = new GlobalDelegates.onCharacterStatusEffectsHaveChanged(this.UpdateStatusEffectsChanged);
            GlobalDelegates.onUniqueIDHasChanged = new GlobalDelegates.onCharacterUniqueIDHasChanged(this.UpdateUniqueIDChanged);
            GlobalDelegates.onMapHasBeenLoaded = new GlobalDelegates.onCharacterMapHasBeenLoaded(this.LoadMapGraph);
            GlobalDelegates.onWorldAggroListChanged = new GlobalDelegates.onWorldAggroListHasChanged(this.UpdateWorldAggroListChanged);

            instance = new Server();
        }

        #region Initialization
        private void start()
        {


            observer = instance.Resolve<IObserver>();

            if (observer == null)
            {
                MessageBox.Show("Observer could not resolve", "Critical error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            gateway = instance.Resolve<IGateway>();

            if (gateway == null)
            {
                MessageBox.Show("Observer could not resolve", "Critical error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

        }

        private void bindCharacter()
        {
            this.Cursor = Cursors.WaitCursor;
            loader.Show();
            loader.BringToFront();

            this.Enabled = false;
            try
            {
                memoryInspector = instance.Resolve<IMemoryInspector>();
                memoryInspector.Start();
            }
            catch (FFXIMemoryObserver.Exceptions.GameNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion

        #region Dipose

        private void Controlboard_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void Controlboard_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                memoryInspector.Terminate();
                observer.Terminate();
                gateway.Terminate();

                if (strategy != null)
                    strategy.Terminate();

                instance.Dispose();
                
                this.Cursor = Cursors.Default;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        
        #endregion

        #region Forms
        private void Controlboard_Load(object sender, EventArgs e)
        {
            try
            {
                start();
                bindCharacter();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), ex.ToString());
                throw ex;
            }
        }


        // Build the Chart
        private void CreateGraph(ZedGraphControl zgc, List<double> lx, List<double> lz)
        {
            // get a reference to the GraphPane
            myPane = zgc.GraphPane;

            // Set the Titles
            if (mapLoaded == Zone.Unknown)
                myPane.Title.Text = "Map loading";
            else
                myPane.Title.Text = mapLoaded.ToString();

            myPane.XAxis.Title.Text = "X";
            myPane.YAxis.Title.Text = "Z";

            Color currentColor = Color.Black;
            //int startidx = 0;

            //if (lx != null && lx.Count > 0)
            //{

            //    if (lx.Count < 100000)
            //    {
            //        LineItem myCurve1 = myPane.AddCurve("", lx.ToArray(), lz.ToArray(), Color.Black, SymbolType.Diamond);
            //        myCurve1.Line.IsVisible = false; // was false for scatter plot
            //        myCurve1.Symbol.Fill = new Fill(Color.Black);
            //    }
            //}

            zgc.AxisChange();
            zgc.Refresh();
        }

        #endregion

        #region Control events
        private void btnBindCharacter_Click(object sender, EventArgs e)
        {
            bindCharacter();
        }

        private void btnGetObject_Click(object sender, EventArgs e)
        {
            //uint count = mapLib.GetObjectMapCount();
            //D3D.ManagedOBJINFO obj = mapLib.GetObjectMap(count - 1);
            //Console.WriteLine(count.ToString());
        }


        private void btnStopRunning_Click(object sender, EventArgs e)
        {
        }

        private void btnApplyStrategy_Click(object sender, EventArgs e)
        {
            if (this.cbStrategy.Text == String.Empty)
            {
                MessageBox.Show("Need to choose a strategy, dork", "Nu.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            this.loader.loadStrategy();
            this.btnApplyStrategy.Enabled = false;
            this.Cursor = Cursors.WaitCursor;
            this.tcCharacters.TabPages.Clear();
            this.cbLeader.Items.Clear();
            this.cbLeader.Text = String.Empty;

            memoryInspector.Terminate();
            Thread.Sleep(100);

            observer.Terminate();
            gateway.Terminate();
            if (strategy != null)
                strategy.Terminate();
            instance.Dispose();


            strategy = this.instance.setStrategy(this.cbStrategy.Text);

            bindCharacter();
            start();


            strategy.ApplyStrategy();

            this.btnApplyStrategy.Enabled = true;
        }

        #endregion

        #region Control Functions
        private void updateListViewElement(string characterName, 
                                           string ListViewName, 
                                           string AttributeName, string AttributeLabel, string AttributeValue)
        {
            foreach (TabPage tp in tcCharacters.TabPages)
            {
                if (tp.Tag.ToString() == characterName)
                {
                    if (tp.Controls.ContainsKey(ListViewName))
                    {
                        ListView lst = (ListView)tp.Controls[ListViewName];
                        if (lst.Items.ContainsKey(AttributeName))
                        {
                            lst.Items[AttributeName].SubItems[1].Text = AttributeValue;
                        }
                        else
                        {
                            ListViewItem attribute = new ListViewItem();
                            attribute.Name = AttributeName;
                            attribute.Text = AttributeLabel;
                            attribute.SubItems.Add(AttributeValue);
                            lst.Items.Add(attribute);
                        }
                    }
                }
            }
        }

        private string getAttribute(string characterName,
                                           string ListViewName,
                                           string AttributeName)
        {
            foreach (TabPage tp in tcCharacters.TabPages)
            {
                if (tp.Text == characterName)
                {
                    if (tp.Controls.ContainsKey(ListViewName))
                    {
                        ListView lst = (ListView)tp.Controls[ListViewName];
                        if (lst.Items.ContainsKey(AttributeName))
                        {
                            string item = lst.Items[AttributeName].SubItems[1].Text;
                            return item;
                        }
                    }
                }
            }
            return String.Empty;
        }
            
        private void updateListViewElement(string characterName,
                                   string ListViewName,
                                   string AttributeName, string AttributeValue)
        {
            foreach (TabPage tp in tcCharacters.TabPages)
            {
                if (tp.Tag.ToString() == characterName)
                {
                    if (tp.Controls.ContainsKey(ListViewName))
                    {
                        ListView lst = (ListView)tp.Controls[ListViewName];
                        if (lst.Items.ContainsKey(AttributeName))
                        {
                            lst.Items[AttributeName].SubItems[0].Text = AttributeValue;
                        }
                        else
                        {
                            ListViewItem attribute = new ListViewItem();
                            attribute.Name = AttributeName;
                            attribute.Text = AttributeValue;
                            lst.Items.Add(attribute);
                        }
                    }
                }
            }
        }

        private void clearListView(string characterName,
                                   string ListViewName)
        {
            foreach (TabPage tp in tcCharacters.TabPages)
            {
                if (tp.Tag.ToString() == characterName)
                {
                    if (tp.Controls.ContainsKey(ListViewName))
                    {
                        ListView lst = (ListView)tp.Controls[ListViewName];
                        lst.Items.Clear();
                    }
                }
            }
        }

        private void toggleVisibility(string characterName,
                                      string controlName,
                                      bool visible)
        {
            foreach (TabPage tp in tcCharacters.TabPages)
            {
                if (tp.Tag.ToString() == characterName)
                {
                    if (tp.Controls.ContainsKey(controlName))
                    {
                        Control ctrl = tp.Controls[controlName];
                        ctrl.Visible = visible;
                    }
                }
            }
        }

        #endregion

        #region Delegates


        public void ToggleEnable(object sender, System.EventArgs e)
        {
            Button senderButton = (Button)sender;

            this.lstEnabledChars.Clear();
            foreach (CharacterAggregateRoot character in this.instance.Resolve<IAggregateRootRepository>().getAggregateList<CharacterAggregateRoot>())
            {
                if (character.characterName == senderButton.Tag.ToString())
                {
                    TabPage tp = null;
                    for (int i = 0; i < this.tcCharacters.TabPages.Count; i++)
                    {
                        if (this.tcCharacters.TabPages[i].Name == character.characterName)
                        {
                            tp = this.tcCharacters.TabPages[i];
                            break;
                        }
                    }

                    if (character.isEnabled)
                    {
                        character.isEnabled = false;
                        senderButton.ForeColor = Color.Green;
                        senderButton.Text = "Enable";
                    }
                    else
                    {
                        character.isEnabled = true;
                        senderButton.ForeColor = Color.Red;
                        senderButton.Text = "Disable";
                    }
                }
                if (character.isEnabled)
                    this.lstEnabledChars.Items.Add(character.characterName);
            }
            

        }

        public void LoadMapGraph(Object _event)
        {
            mapLib = (D3DMap)_event;
        }

        #region Character Delegates

        public void UpdateAvailable(Object _event)
        {
            CharacterAvailableForActionEvent domain =  (CharacterAvailableForActionEvent)_event;

            this.tcCharacters.Invoke(new Action(delegate()
            {
                this.updateListViewElement(domain.characterName.ToString(), "lstAttribute", "attIsBusy", "Current Action", "Available, Success: " + domain.isSuccess.ToString());
            }));
        }

        public void UpdateHasBeenInitialized(Object _event)
        {
            CharacterHasBeenInitializedEvent domain = (CharacterHasBeenInitializedEvent)_event;


            this.tcCharacters.Invoke( new Action(delegate()
            {
                tcCharacters.TabPages.Add(domain.characterName, domain.characterName);

                foreach (TabPage tp in tcCharacters.TabPages)
                {
                    if (tp.Text == domain.characterName)
                    {
                        tp.Tag = domain.characterName;

                        ListView lstView = new ListView();
                        lstView.Name = "lstAttribute";
                        lstView.Columns.Add("Attribute").Width = 150;
                        lstView.Columns.Add("Value").Width = 150;
                        lstView.Width = 306;
                        lstView.Height = 350;
                        lstView.View = View.Details;
                        //lstView.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
                        lstView.Location = new Point(650, 25);

                        
                        ListView lstViewTarget = new ListView();
                        lstViewTarget.Name = "lstTarget";
                        lstViewTarget.Columns.Add("Attribute").Width = 150;
                        lstViewTarget.Columns.Add("Value").Width = 150;
                        lstViewTarget.Width = 306;
                        lstViewTarget.Height = 175;
                        lstViewTarget.View = View.Details;
                        lstViewTarget.Location = new Point(320, 15);

                        ListView lstViewStatusEffects = new ListView();
                        lstViewStatusEffects.Name = "lstStatusEffects";
                        lstViewStatusEffects.Columns.Add("Effect").Width = 305;
                        lstViewStatusEffects.Width = 306;
                        lstViewStatusEffects.Height = 195;
                        lstViewStatusEffects.View = View.Details;
                        lstViewStatusEffects.Location = new Point(320, 200);

                        tp.Controls.Add(lstView);
                        tp.Controls.Add(lstViewStatusEffects);
                        tp.Controls.Add(lstViewTarget);

                        cbLeader.Items.Add(domain.characterName);
                        cbLeader.SelectedIndex = 0;

                        Button btnEnable = new Button();
                        btnEnable.Name = "btnEnable";
                        btnEnable.Text = "Enable character";
                        btnEnable.Tag = domain.characterName;
                        btnEnable.Location = new Point(30, 30);
                        btnEnable.Click += new EventHandler(ToggleEnable);
                        btnEnable.ForeColor = Color.Green;
                        tp.ForeColor = Color.Red;
                        tp.Controls.Add(btnEnable);
                    }
                }
                
            }));
                
        }

        public void UpdateMap(Object _event)
        {
            CharacterHasChangedMapEvent domain = (CharacterHasChangedMapEvent)_event;

            this.tcCharacters.Invoke(new Action(delegate()
            {
                
                this.updateListViewElement(domain.characterName.ToString(), "lstAttribute", "attMap", "Current Map", domain.mapId.ToString() + ((int)domain.mapId).ToString());
            }));



            mapLoaded = domain.mapId;



            this.tcCharacters.Invoke(new Action(delegate()
            {

                myPane.Title.Text = domain.mapId.ToString();
                this.Enabled = true;
                this.Cursor = Cursors.Default;
                loader.Hide();
                this.BringToFront();
            }));
        }

        public void UpdateDisconnected(Object _event)
        {
            CharacterHasDisconnectedEvent domain = (CharacterHasDisconnectedEvent)_event;

            this.tcCharacters.Invoke(new Action(delegate()
            {
                tcCharacters.TabPages.Remove(tcCharacters.TabPages[domain.characterName]);
            }));
        }

        public void UpdateMoved(Object _event)
        {
            CharacterHasMovedEvent domain = (CharacterHasMovedEvent)_event;

            this.tcCharacters.Invoke(new Action(delegate()
            {
                this.updateListViewElement(domain.characterName.ToString(), "lstAttribute", "attX", "X", domain.newX.ToString());
                this.updateListViewElement(domain.characterName.ToString(), "lstAttribute", "attY", "Y", domain.newY.ToString());
                this.updateListViewElement(domain.characterName.ToString(), "lstAttribute", "attZ", "Z", domain.newZ.ToString());
                this.updateListViewElement(domain.characterName.ToString(), "lstAttribute", "attH", "H", domain.newFacing.ToString());
            }));
        }

        public void UpdateHPChanged(Object _event)
        {
            CharacterHPHasChangedEvent domain = (CharacterHPHasChangedEvent)_event;

            this.tcCharacters.Invoke(new Action(delegate()
            {
                 this.updateListViewElement(domain.characterName.ToString(), "lstAttribute", "attHP", "Current HP", domain.hp.ToString());
            }));
        }

        public void UpdateBusyWithAction(Object _event)
        {
            CharacterIsBusyWithActionEvent domain = (CharacterIsBusyWithActionEvent)_event;

            this.tcCharacters.Invoke(new Action(delegate()
            {
                this.updateListViewElement(domain.characterName.ToString(), "lstAttribute", "attIsBusy", "Current Action", "Busy");
            }));
        }



        public void UpdateMPChanged(Object _event)
        {
            CharacterMPHasChangedEvent domain = (CharacterMPHasChangedEvent)_event;

            this.tcCharacters.Invoke(new Action(delegate()
            {
                this.updateListViewElement(domain.characterName.ToString(), "lstAttribute", "attMP", "Current MP", domain.mp.ToString());
            }));
        }

        public void UpdateTPChanged(Object _event)
        {
            CharacterTPHasChangedEvent domain = (CharacterTPHasChangedEvent)_event;

            this.tcCharacters.Invoke(new Action(delegate()
            {
                this.updateListViewElement(domain.characterName.ToString(), "lstAttribute", "attTP", "Current TP", domain.tp.ToString());
            }));
        }


        public void UpdateChangedTarget(Object _event)
        {
            CharacterHasChangedTargetEvent domain = (CharacterHasChangedTargetEvent)_event;

            this.tcCharacters.Invoke(new Action(delegate()
            {
                this.updateListViewElement(domain.characterName.ToString(), "lstAttribute", "attTarget", "Current Target", domain.target.Name.ToString());

                clearListView(domain.characterName.ToString(), "lstTarget");
                if (domain.target.ID > 0)
                {
                    toggleVisibility(domain.characterName.ToString(), "lstTarget", true);
                    this.updateListViewElement(domain.characterName.ToString(), "lstTarget", "attID", "ID", domain.target.ID.ToString());
                    this.updateListViewElement(domain.characterName.ToString(), "lstTarget", "attSubID", "SubID", domain.target.SubID.ToString());
                    this.updateListViewElement(domain.characterName.ToString(), "lstTarget", "attServerID", "ServerID", domain.target.ServerID.ToString());
                    this.updateListViewElement(domain.characterName.ToString(), "lstTarget", "attSubServerID", "SubServerID", domain.target.SubServerID.ToString());

                    this.updateListViewElement(domain.characterName.ToString(), "lstTarget", "attID", "Is Locked", domain.target.IsLocked.ToString());
                    this.updateListViewElement(domain.characterName.ToString(), "lstTarget", "attID", "Is Sub", domain.target.IsSub.ToString());

                    this.updateListViewElement(domain.characterName.ToString(), "lstTarget", "attPosX", "X", domain.target.PosX.ToString());
                    this.updateListViewElement(domain.characterName.ToString(), "lstTarget", "attPosY", "Y", domain.target.PosY.ToString());
                    this.updateListViewElement(domain.characterName.ToString(), "lstTarget", "attPosZ", "Z", domain.target.PosZ.ToString());
                    this.updateListViewElement(domain.characterName.ToString(), "lstTarget", "attPosH", "H", domain.target.PosH.ToString());

                    this.updateListViewElement(domain.characterName.ToString(), "lstTarget", "Object Type", domain.target.Type.ToString());
                }
                else
                {
                    toggleVisibility(domain.characterName.ToString(), "lstTarget", false);
                }
            }));
        }

        public void UpdateCastingProgress(Object _event)
        {
            CharacterCastProgressChangedEvent domain = (CharacterCastProgressChangedEvent)_event;

            this.tcCharacters.Invoke(new Action(delegate()
            {
                this.updateListViewElement(domain.characterName.ToString(), "lstAttribute", "attCast", "Cast progress", domain.newCastProgress.ToString() + "%");
            }));
        }

        public void UpdateLoginStatus(Object _event)
        {
            CharacterLoginStatusChangedEvent domain = (CharacterLoginStatusChangedEvent)_event;

            this.tcCharacters.Invoke(new Action(delegate()
            {
                this.updateListViewElement(domain.characterName.ToString(), "lstAttribute", "attLoginStatus", "Login status", domain.loginStatus.ToString());
            }));
        }

        public void UpdateNameChanged(Object _event)
        {
            CharacterNameChangedEvent domain = (CharacterNameChangedEvent)_event;

            this.tcCharacters.Invoke(new Action(delegate()
            {
                this.tcCharacters.TabPages[this.tcCharacters.TabPages.IndexOfKey(domain.oldCharacterName)].Text = domain.characterName;
            }));
        }

        public void UpdateViewModeChanged(Object _event)
        {
            CharacterViewModeChangedEvent domain = (CharacterViewModeChangedEvent)_event;

            this.tcCharacters.Invoke(new Action(delegate()
            {
                this.updateListViewElement(domain.characterName.ToString(), "lstAttribute", "attViewMode", "Viewmode", domain.viewMode.ToString());
            }));
        }

        public void UpdateStatusChanged(Object _event)
        {
            CharacterStatusHasChangedEvent domain = (CharacterStatusHasChangedEvent)_event;

            this.tcCharacters.Invoke(new Action(delegate()
            {
                this.updateListViewElement(domain.characterName.ToString(), "lstAttribute", "attStatus", "Status", domain.status.ToString());
            }));
        }

        public void UpdateWorldAggroListChanged(Object _event)
        {
            ITruthRepository world = this.instance.Resolve<ITruthRepository>();

            
            this.tcCharacters.Invoke(new Action(delegate()
            {
                List<AggroShard> aggros = world.getAllAggros();
                lstAggro.Items.Clear();

                foreach (AggroShard shard in aggros)
                {
                    lstAggro.Items.Add(shard.aggroedObjectId + "<-" + shard.ID);
                }

            }));
        }

        public void UpdateStatusEffectsChanged(Object _event)
        {
            CharacterStatusEffectsHaveChangedEvent domain = (CharacterStatusEffectsHaveChangedEvent)_event;

            this.tcCharacters.Invoke(new Action(delegate()
            {
                clearListView(domain.characterName.ToString(), "lstStatusEffects");
                if (domain.statusEffects.Length > 0)
                {
                    toggleVisibility(domain.characterName.ToString(), "lstStatusEffects", true);
                    foreach (StatusEffect effect in domain.statusEffects)
                    {
                        if (effect != StatusEffect.Unknown)
                            this.updateListViewElement(domain.characterName.ToString(), "lstStatusEffects", "att" + effect.ToString(), effect.ToString());
                    }
                }
                else
                {
                    toggleVisibility(domain.characterName.ToString(), "lstStatusEffects", false);
                }
            }));
        }

        public void UpdateUniqueIDChanged(Object _event)
        {
            CharacterUniqueIDHasChanged domain = (CharacterUniqueIDHasChanged)_event;

            this.tcCharacters.Invoke(new Action(delegate()
            {
                this.updateListViewElement(domain.characterName.ToString(), "lstAttribute", "attUniqueID", "Unique ID", domain.uniqueID.ToString());
            }));
        }
                    
        #endregion

        private void zGraph_Load(object sender, EventArgs e)
        {

        }


        private void zGraph_MouseClick(object sender, MouseEventArgs e)
        {
            //if (this.mapLoaded  != Zone.Unknown && mapLib != null)
            //{
            //    double[] closest = new double[3];
            //    closest[0] = Double.Parse(getAttribute(cbLeader.Text, "lstAttribute", "attX"));
            //    closest[1] = Double.Parse(getAttribute(cbLeader.Text, "lstAttribute", "attY"));
            //    closest[2] = Double.Parse(getAttribute(cbLeader.Text, "lstAttribute", "attZ"));


            //    int currentX = (int)closest[0];
            //    int currentZ = (int)closest[2];

            //    Point currentPoint = mapLib.convertToGridSquare(currentX, currentZ);
            //    int[] currentPt = mapLib.getClosest(new int[] { currentPoint.X, currentPoint.Y });
            //    currentPoint.X = currentPt[0];
            //    currentPoint.Y = currentPt[1];

            //    myPane.ReverseTransform(e.Location, out closest[0], out closest[2]);

            //    int newX = (int)closest[0];
            //    int newZ = (int)closest[2];

            //    Point newPoint = mapLib.convertToGridSquare(newX, newZ);
            //    int[] newPt = mapLib.getClosest(new int[] { newPoint.X, newPoint.Y });
            //    newPoint.X = newPt[0];
            //    newPoint.Y = newPt[1];

            //    Color currentColor = Color.Black;
            //    //int startidx = 0;

            //    if (mapLib.getCoordinatesX() != null && mapLib.getCoordinatesX().Count > 0)
            //    {
            //        List<double> pathx;
            //        List<double> pathz;

            //        List<int[]> nodes = mapLib.getPath(currentPoint, newPoint, out pathx, out pathz);



            //        myPane.CurveList.Clear();
            //        myPane.GraphObjList.Clear();

            //        if (nodes != null)
            //        {
            //            LineItem myCurve2 = myPane.AddCurve("", pathx.ToArray(), pathz.ToArray(), Color.Red, SymbolType.Circle);
            //            myCurve2.Line.IsVisible = false; // was false for scatter plot
            //            myCurve2.Symbol.Fill = new Fill(Color.Red);
            //        }
            //        LineItem myCurve1 = myPane.AddCurve("", mapLib.getCoordinatesX().ToArray(), mapLib.getCoordinatesZ().ToArray(), Color.Black, SymbolType.Diamond);
            //        myCurve1.Line.IsVisible = false; // was false for scatter plot
            //        myCurve1.Symbol.Fill = new Fill(Color.Black);

            //        zGraph.Refresh();
            //    }
            //}
        }

        #endregion

        private void cbLeader_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cbLeader.Text != String.Empty)
            {
                IAggregateRootRepository root = instance.Resolve<IAggregateRootRepository>();

                foreach (CharacterAggregateRoot character in root.getAggregateList<CharacterAggregateRoot>())
                {
                    if (character.characterName == this.cbLeader.Text)
                    {
                        character.isBotLeader = true;
                        this.memoryInspector.setLeaderId(character.characterName);
                    }
                    else
                        character.isBotLeader = false;
                }
            }
        }




       

    }
}
