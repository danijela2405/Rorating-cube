using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using OpenGL4NET;

namespace Rotate
{
    class Cube : Form, IDisposable
    {

        RenderingContext rc;
        Program g_program;
        Matrix g_projection;
        Matrix g_modelView;

        int g_projectionLocation;
        int g_modelViewLocation;
        uint g_vao;
        uint g_vertices;
        uint g_normals;
        uint g_indices;
        uint g_vertexLocation;
        uint g_normalLocation;

        public bool Init()
        {


            #region vrhovi kocke
            float[] points = new float[] { 
                          -0.5f, -0.5f, -0.5f, 1.0f,
						  -0.5f, -0.5f, +0.5f, 1.0f,
						  +0.5f, -0.5f, +0.5f, 1.0f,
						  +0.5f, -0.5f, -0.5f, 1.0f,
						  -0.5f, +0.5f, -0.5f, 1.0f,
						  -0.5f, +0.5f, +0.5f, 1.0f,
						  +0.5f, +0.5f, +0.5f, 1.0f,
						  +0.5f, +0.5f, -0.5f, 1.0f,
						  -0.5f, -0.5f, -0.5f, 1.0f,
						  -0.5f, +0.5f, -0.5f, 1.0f,
						  +0.5f, +0.5f, -0.5f, 1.0f,
						  +0.5f, -0.5f, -0.5f, 1.0f,
						  -0.5f, -0.5f, +0.5f, 1.0f,
						  -0.5f, +0.5f, +0.5f, 1.0f,
						  +0.5f, +0.5f, +0.5f, 1.0f,
						  +0.5f, -0.5f, +0.5f, 1.0f,
						  -0.5f, -0.5f, -0.5f, 1.0f,
						  -0.5f, -0.5f, +0.5f, 1.0f,
						  -0.5f, +0.5f, +0.5f, 1.0f,
						  -0.5f, +0.5f, -0.5f, 1.0f,
						  +0.5f, -0.5f, -0.5f, 1.0f,
						  +0.5f, -0.5f, +0.5f, 1.0f,
						  +0.5f, +0.5f, +0.5f, 1.0f,
						  +0.5f, +0.5f, -0.5f, 1.0f
			};
            #endregion

            #region Normale kocke
            float[] normals = new float[] {
                          +0.0f, -1.0f, +0.0f,
						  +0.0f, -1.0f, +0.0f,
						  +0.0f, -1.0f, +0.0f,
						  +0.0f, -1.0f, +0.0f,
						  +0.0f, +1.0f, +0.0f,
						  +0.0f, +1.0f, +0.0f,
						  +0.0f, +1.0f, +0.0f,
						  +0.0f, +1.0f, +0.0f,
						  +0.0f, +0.0f, -1.0f,
						  +0.0f, +0.0f, -1.0f,
						  +0.0f, +0.0f, -1.0f,
						  +0.0f, +0.0f, -1.0f,
						  +0.0f, +0.0f, +1.0f,
						  +0.0f, +0.0f, +1.0f,
						  +0.0f, +0.0f, +1.0f,
						  +0.0f, +0.0f, +1.0f,
						  -1.0f, +0.0f, +0.0f,
						  -1.0f, +0.0f, +0.0f,
						  -1.0f, +0.0f, +0.0f,
						  -1.0f, +0.0f, +0.0f,
						  +1.0f, +0.0f, +0.0f,
						  +1.0f, +0.0f, +0.0f,
						  +1.0f, +0.0f, +0.0f,
						  +1.0f, +0.0f, +0.0f
			};
            #endregion

            #region indeksi
            uint[] indices = {		
                                0, 2, 1,
								0, 3, 2, 
								4, 5, 6,
								4, 6, 7,
								8, 9, 10,
								8, 10, 11, 
								12, 15, 14,
								12, 14, 13, 
								16, 17, 18,
								16, 18, 19, 
								20, 23, 22,
								20, 22, 21
			};
            #endregion

            DoubleBuffered = true;
            ClientSize = new Size(600, 600);
            Text = "OpenGL 3.2 - Example 03";
            Resize += new EventHandler(WindowResize);

            rc = RenderingContext.CreateContext(this, new RenderingContextSetting()
            {
                majorVersion = 3,
                minorVersion = 2,
                profile = RenderingContextSetting.ProfileEnum.Core,
                context = RenderingContextSetting.ContextEnum.ForwardCompatible
            }
            );
            if (!gl.Extension.isVersionOrHihger(3, 2))
            {
                Console.WriteLine("OpenGL 3.2 not supported.");
                return false;
            }

            // matrica za model
            Matrix model;

            g_program = new Program("Basic", File.ReadAllText("Zadatak3.vert"), File.ReadAllText("Zadatak3.frag"));
            Console.WriteLine(g_program.log);

            g_vao = gl.GenVertexArray();
            gl.BindVertexArray(g_vao);

            g_projectionLocation = gl.GetUniformLocation(g_program.id, "projectionMatrix");

            g_modelViewLocation = gl.GetUniformLocation(g_program.id, "modelViewMatrix");

            g_vertexLocation = (uint)gl.GetAttribLocation(g_program.id, "vertex");

            g_normalLocation = (uint)gl.GetAttribLocation(g_program.id, "normal");

            gl.BindFragDataLocation(g_program.id, 0, "fragColor");

            g_vertices = gl.GenBuffer();

            gl.BindBuffer(GL.ARRAY_BUFFER, g_vertices);

            gl.BufferData(GL.ARRAY_BUFFER, 24 * 4 * 4, points, GL.STATIC_DRAW);

            g_normals = gl.GenBuffer();

            gl.BindBuffer(GL.ARRAY_BUFFER, g_normals);

            gl.BufferData(GL.ARRAY_BUFFER, 24 * 3 * 4, normals, GL.STATIC_DRAW);

            g_indices = gl.GenBuffer();

            gl.BindBuffer(GL.ELEMENT_ARRAY_BUFFER, g_indices);

            gl.BufferData(GL.ELEMENT_ARRAY_BUFFER, 6 * 2 * 3 * 4, indices, GL.STATIC_DRAW);

            gl.UseProgram(g_program.id);


            // izračunavanje matrice modela
            model = Matrix.RotationZYX(30.0f, 30.0f, 0.0f);
            // izračunavanje view matrice
            g_modelView = Matrix.LookAt(0.0f, 0.0f, 5.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f);
            // model view matrica
            g_modelView.Mult(ref model);

            gl.UniformMatrix4fv(g_modelViewLocation, 1, false, ref g_modelView);

            gl.BindBuffer(GL.ARRAY_BUFFER, g_vertices);

            gl.VertexAttribPointer(g_vertexLocation, 4, GL.FLOAT, false, 0, IntPtr.Zero);

            gl.EnableVertexAttribArray(g_vertexLocation);

            gl.BindBuffer(GL.ARRAY_BUFFER, g_normals);

            gl.VertexAttribPointer(g_normalLocation, 3, GL.FLOAT, false, 0, IntPtr.Zero);

            gl.EnableVertexAttribArray(g_normalLocation);

            gl.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);

            gl.ClearDepth(1.0f);
            gl.Enable(GL.DEPTH_TEST);
            gl.Enable(GL.CULL_FACE);

            WindowResize(null, null);
            return true;
        }

        void WindowResize(object sender, EventArgs e)
        {

            gl.Viewport(0, 0, ClientSize.Width, ClientSize.Height);

            g_projection = Matrix.Perspective(40.0f, (float)ClientSize.Width / (float)ClientSize.Height, 1.0f, 100.0f);

            gl.UniformMatrix4fv(g_projectionLocation, 1, false, ref g_projection);

        }

        //renderiranje
        void Render()
        {
            gl.Clear(GL.COLOR_BUFFER_BIT | GL.DEPTH_BUFFER_BIT);
            gl.DrawElements(GL.TRIANGLES, 6 * 2 * 3, GL.UNSIGNED_INT, IntPtr.Zero);

            rc.SwapBuffers();
        }

        //brisanje
        protected override void Dispose(bool disposing)
        {
            if (g_vertices != 0)
            {
                gl.DeleteBuffer(g_vertices);

                g_vertices = 0;
            }

            if (g_normals != 0)
            {
                gl.DeleteBuffer(g_normals);

                g_normals = 0;
            }

            if (g_indices != 0)
            {
                gl.DeleteBuffer(g_indices);

                g_indices = 0;
            }

            gl.DeleteVertexArray(g_vao);

            g_program.Delete();

            base.Dispose(disposing);
        }


        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case Windows.WM_PAINT: Render();
                    break;
                default: base.WndProc(ref m);
                    break;
            }
        }


        static void Main(string[] args)
        {
            Zadatak3 form = new Zadatak3();
            if (form.Init())
                Application.Run(form);
            form.Dispose();
        }
    }
    
}
