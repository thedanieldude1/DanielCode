using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using NeuralNetwork;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
namespace NeuralNetworkSimulator
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public static int Width;
        public static int Height;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public List<Agent> Agents;
        public List<Rectangle> AgentsBoxes;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.SynchronizeWithVerticalRetrace = false;
            Width =  graphics.GraphicsDevice.Viewport.Width;
            Height =  graphics.GraphicsDevice.Viewport.Height;
            // TODO: Add your initialization logic here
            Agents = new List<Agent>();
            // for(int i = 0; i < 5; i++)
            // {
            //Agents.Add(new Agent(new Random()));
            // }
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        float BestFit = 0;
        float SecondBestFit = 0;
        float ThirdBestFit = 0;
        int steps = 1;
        bool justpressedA = false;
        Network BestBrain;
        Network secondBestBrain;
        Network thirdBestBrain;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 
        bool isPaused = false;
        bool firstSpawned = false;
        protected override void Update(GameTime gameTime)
        {
            this.TargetElapsedTime = TimeSpan.FromMilliseconds(2f);
            
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if (Agents.Count < 20)
            {
                if (BestBrain != null&&secondBestBrain!=null)
                {
                    Agent a = new Agent(new Random(), this);
                    string rep = GenomeEncoder.Encode(BestBrain);
                    //Console.WriteLine(rep.Length / 32);
                    string rep2 = GenomeEncoder.Encode(secondBestBrain);
                    string dr = GenomeEncoder.Combine(rep, rep2);
                    //Console.WriteLine(dr);
                    a.brain = GenomeEncoder.Decode(dr);
                    //Console.WriteLine(dr.Length - rep.Length);
                    Agents.Add(a);
                }
                else if (!firstSpawned)
                {
                    Agents.Add(new Agent(new Random(), this));
                }
            }
            else
            {
                if (!firstSpawned)
                {
                    firstSpawned = true;
                }
            }
            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.B))
            {
                if (!justpressedA){
                    isPaused = !isPaused;
                    if (isPaused)
                    {
                        Console.WriteLine("Current Best Fitness: " + BestFit);
                        XmlSerializer ser = new XmlSerializer(typeof(Network));
                        Console.WriteLine(BestBrain.isTraining);
                        TextWriter writer = new StreamWriter(@"Ihateyou.txt");
                        ser.Serialize(writer, BestBrain);
                        string all = GenomeEncoder.Encode(BestBrain);
                        //Console.WriteLine(.Length/32);
                        Network x = GenomeEncoder.Decode(all);
                        //if(x.HiddenLayer1[2].TargetSynapses[0].Weight==BestBrain.HiddenLayer1[2].TargetSynapses[0].Weight&& x.Inputs[2].TargetSynapses[0].Weight == BestBrain.Inputs[2].TargetSynapses[0].Weight && x.HiddenLayer2[2].RecursiveSynapses[0].Weight == BestBrain.HiddenLayer2[2].RecursiveSynapses[0].Weight)
                       // {
                       //     Console.WriteLine("I guess they are equal I guess");
                        //}
                       // byte[] r = BitConverter.GetBytes(0.34f);
                       // BitArray a = new BitArray(r);
                        //Console.WriteLine(BitConverter.ToSingle(GenomeEncoder.ConvertToByte(a),0));
                    }
                    justpressedA = true;
                }
            }
            else
            {
                justpressedA = false;
            }
            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.A))
            {
                steps = 1;
                Console.WriteLine("B Pressed");
            }
            if (!isPaused) { 
            
                for (int x = 0; x < steps;x++){

                    for (int i = 0; i < Agents.Count; i++)
                    {
                        var n = Agents[i];
                        if (n.Health <= 0)
                        {
                            //if (n.Dead != true)
                            // {
                            //     Agents.Add(new Agent(new Random(), this));
                            //    n.Dead = true;
                            // }
                            //Console.WriteLine(i + "'s fitness was " + n.Fitness);
                            if (n.Fitness > BestFit) {
                                secondBestBrain = BestBrain;
                                SecondBestFit = BestFit;
                                BestFit = n.Fitness;
                                BestBrain = n.brain;
                                
                            }
                            else if(n.Fitness>SecondBestFit)
                            {
                                secondBestBrain = n.brain;
                                SecondBestFit = n.Fitness;
                                //Console.WriteLine("Found second: " + n.Fitness);
                            }
                            
                            Agents.Remove(n);
                            continue;

                        }

                        n.Tick();

                    }
                }
            }
            // TODO: Add your update logic here
            //Angle = (Angle + 1)%360;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            Texture2D rect = new Texture2D(graphics.GraphicsDevice, 40, 15);

            Color[] data = new Color[40 * 15];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.Chocolate;
            rect.SetData(data);
            foreach (Agent n in Agents)
            {
                if (n.Health <= 0)
                {
                    continue;
                }//n.Angle * (float)(Math.PI / 180)
                spriteBatch.Draw(rect, n.Pos, n.Box, new Color(255,255,255,n.Health/100), 0, new Vector2(20, 7.5f), 1f, SpriteEffects.None, 0f);
                
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
