using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using System.IO;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Cube0005
{

    using System.Windows.Forms;
    public partial class Form1 : Form
    {
        private GLControl glControl;
        private Camera camera;
        private float deltaTime;
        private float lastFrame;

        private int vertexArray;
        private int vertexBuffer;
        private Shader shader;
        private int lineVertexArray;
        private int lineVertexBuffer;

        public Form1()
        {
            InitializeComponent();
            // Создаем и настраиваем GLControl
            glControl = new GLControl(new GraphicsMode(new ColorFormat(32), 24, 0, 4), 3, 3, GraphicsContextFlags.ForwardCompatible)
            {
                Dock = DockStyle.Fill
            };


            glControl.Load += GLControl_Load;
            glControl.Paint += GLControl_Paint;
            glControl.Resize += GLControl_Resize;
            glControl.MouseMove += GLControl_MouseMove;
            glControl.MouseWheel += GLControl_MouseWheel;
            this.Controls.Add(glControl);

            camera = new Camera(new Vector3(0.0f, 0.0f, 3.0f), Vector3.UnitY, -90.0f, 0.0f);

            Application.Idle += Application_Idle; // For continuous rendering
        }



        private void GLControl_Load(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, glControl.Width, glControl.Height);
            GL.Enable(EnableCap.DepthTest);


           GL.ClearColor(Color4.CornflowerBlue);
           GL.Enable(EnableCap.DepthTest);

            // Установка данных вершин
            float[] vertices = {
               0.0f,  0.8f, 0.0f,  // Верхняя вершина
              -0.8f, -0.8f, 0.0f,  // Нижняя левая вершина
               0.8f, -0.8f, 0.0f   // Нижняя правая вершина
             };

            vertexArray = GL.GenVertexArray();
            vertexBuffer = GL.GenBuffer();

            GL.BindVertexArray(vertexArray);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);


            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            string glVersion = GL.GetString(StringName.Version);
            string glslVersion = GL.GetString(StringName.ShadingLanguageVersion);
            Console.WriteLine($"OpenGL version: {glVersion}");
            Console.WriteLine($"GLSL version: {glslVersion}");
            float[] bufferData = new float[vertices.Length];
            GL.GetBufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, bufferData.Length * sizeof(float), bufferData);

            // Вывод содержимого буфера в консоль
            Console.WriteLine("Vertex Buffer Data: " + string.Join(", ", bufferData));

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);


            // Загрузка и компиляция шейдеров
            try
            {
                shader = new Shader("vertex_shader.glsl", "fragment_shader.glsl");
                //shader.Use();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Shader Compilation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            // Установка данных вершин для линии
            float[] lineVertices = {
        0.0f, 0.0f, 0.0f,  // Первая точка (x, y, z)
        1.0f, 1.0f, 0.0f   // Вторая точка (x, y, z)
    };

             lineVertexArray = GL.GenVertexArray();
             lineVertexBuffer = GL.GenBuffer();

            GL.BindVertexArray(lineVertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, lineVertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, lineVertices.Length * sizeof(float), lineVertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Очистка буфера цвета и глубины при каждой отрисовке
            GL.ClearColor(Color4.CornflowerBlue);
            GL.Enable(EnableCap.DepthTest);
        }

        private void Render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Использование шейдера для треугольника
            GL.UseProgram(shader.Handle);


            Matrix4 model = Matrix4.Identity;
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(camera.GetZoom()), glControl.Width / (float)glControl.Height, 0.1f, 100.0f);

            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", view);
            shader.SetMatrix4("projection", projection);

            GL.BindVertexArray(vertexArray);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            // Рендеринг линии
            GL.BindVertexArray(lineVertexArray);
            GL.DrawArrays(PrimitiveType.Lines, 0, 2);


            // Проверка наличия ошибок OpenGL
            ErrorCode error = GL.GetError();
            if (error != ErrorCode.NoError)
            {
                Console.WriteLine("OpenGL error: " + error);
                // Обработка ошибки, если необходимо
            }


            glControl.SwapBuffers();


        }



        private void GLControl_Paint(object sender, PaintEventArgs e)
        {
            Render();
        }

        private void GLControl_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, glControl.Width, glControl.Height);
        }

        private void GLControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                float xOffset = e.X - glControl.Width / 2.0f;
                float yOffset = glControl.Height / 2.0f - e.Y;
                camera.ProcessMouseMovement(xOffset, yOffset);
                Cursor.Position = PointToScreen(new Point(glControl.Width / 2, glControl.Height / 2));
            }
        }

        private void GLControl_MouseWheel(object sender, MouseEventArgs e)
        {
            camera.ProcessMouseScroll(e.Delta);
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            while (glControl.IsIdle)
            {
                float currentFrame = (float)System.Environment.TickCount / 1000.0f;
                deltaTime = currentFrame - lastFrame;
                lastFrame = currentFrame;

                ProcessInput();
                Render();
                glControl.SwapBuffers();
            }
        }

        private void ProcessInput()
        {
            var state = Keyboard.GetState();

            if (state.IsKeyDown(Key.Escape))
                Close();

            if (state.IsKeyDown(Key.W))
                camera.ProcessKeyboard(Key.W, deltaTime);
            if (state.IsKeyDown(Key.S))
                camera.ProcessKeyboard(Key.S, deltaTime);
            if (state.IsKeyDown(Key.A))
                camera.ProcessKeyboard(Key.A, deltaTime);
            if (state.IsKeyDown(Key.D))
                camera.ProcessKeyboard(Key.D, deltaTime);
            if (state.IsKeyDown(Key.Space))
                camera.ProcessKeyboard(Key.Space, deltaTime);
            if (state.IsKeyDown(Key.ControlLeft))
                camera.ProcessKeyboard(Key.ControlLeft, deltaTime);
        }

    }

    public class Shader
    {
        public int Handle;

        public Shader(string vertexPath, string fragmentPath)
        {
            int VertexShader;
            int FragmentShader;

            string VertexShaderSource = File.ReadAllText(vertexPath);
            string FragmentShaderSource = File.ReadAllText(fragmentPath);

            // Compile vertex shader
            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);
            GL.CompileShader(VertexShader);
            CheckShaderCompileStatus(VertexShader);

            // Compile fragment shader
            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);
            GL.CompileShader(FragmentShader);
            CheckShaderCompileStatus(FragmentShader);

            // Link shaders
            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, VertexShader);
            GL.AttachShader(Handle, FragmentShader);
            GL.LinkProgram(Handle);
            CheckProgramLinkStatus(Handle);

            // Cleanup
            GL.DetachShader(Handle, VertexShader);
            GL.DetachShader(Handle, FragmentShader);
            GL.DeleteShader(VertexShader);
            GL.DeleteShader(FragmentShader);
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        public void SetMatrix4(string name, Matrix4 matrix)
        {
            int location = GL.GetUniformLocation(Handle, name);
            if (location == -1)
            {
                Console.WriteLine($"ERROR::SHADER::UNIFORM::{name} NOT FOUND");
                throw new Exception($"ERROR::SHADER::UNIFORM::{name} NOT FOUND");
            }
            GL.UniformMatrix4(location, true, ref matrix);
        }

        private void CheckShaderCompileStatus(int shader)
        {
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                Console.WriteLine($"ERROR::SHADER::COMPILATION_FAILED\n{infoLog}");
                throw new Exception($"ERROR::SHADER::COMPILATION_FAILED\n{infoLog}");
            }
        }

        private void CheckProgramLinkStatus(int program)
        {
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(program);
                Console.WriteLine($"ERROR::SHADER::PROGRAM::LINKING_FAILED\n{infoLog}");
                throw new Exception($"ERROR::SHADER::PROGRAM::LINKING_FAILED\n{infoLog}");
            }
        }
    }

    public class Camera
    {
        public Vector3 Position { get; private set; }
        public Vector3 Front { get; private set; }
        public Vector3 Up { get; private set; }
        public Vector3 Right { get; private set; }
        public Vector3 WorldUp { get; private set; }

        private float Yaw;
        private float Pitch;

        private float MovementSpeed;
        private float MouseSensitivity;
        private float Zoom;

        public Camera(Vector3 position, Vector3 up, float yaw, float pitch)
        {
            Position = position;
            WorldUp = up;
            Yaw = yaw;
            Pitch = pitch;
            Front = new Vector3(0.0f, 0.0f, -1.0f);
            MovementSpeed = 2.5f;
            MouseSensitivity = 0.1f;
            Zoom = 45.0f;

            UpdateCameraVectors();
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + Front, Up);
        }

        public void ProcessKeyboard(Key key, float deltaTime)
        {
            float velocity = MovementSpeed * deltaTime;
            if (key == Key.W)
                Position += Front * velocity;
            if (key == Key.S)
                Position -= Front * velocity;
            if (key == Key.A)
                Position -= Right * velocity;
            if (key == Key.D)
                Position += Right * velocity;
            if (key == Key.Space)
                Position += Up * velocity;
            if (key == Key.ControlLeft)
                Position -= Up * velocity;
        }

        public void ProcessMouseMovement(float xOffset, float yOffset, bool constrainPitch = true)
        {
            xOffset *= MouseSensitivity;
            yOffset *= MouseSensitivity;

            Yaw += xOffset;
            Pitch -= yOffset; // Inverted controls

            if (constrainPitch)
            {
                if (Pitch > 89.0f)
                    Pitch = 89.0f;
                if (Pitch < -89.0f)
                    Pitch = -89.0f;
            }

            UpdateCameraVectors();
        }

        public void ProcessMouseScroll(float yOffset)
        {
            Zoom -= yOffset;
            if (Zoom < 1.0f)
                Zoom = 1.0f;
            if (Zoom > 45.0f)
                Zoom = 45.0f;
        }

        public float GetZoom()
        {
            return Zoom;
        }

        private void UpdateCameraVectors()
        {
            // Calculate the new Front vector
            Vector3 front;
            front.X = (float)Math.Cos(MathHelper.DegreesToRadians(Yaw)) * (float)Math.Cos(MathHelper.DegreesToRadians(Pitch));
            front.Y = (float)Math.Sin(MathHelper.DegreesToRadians(Pitch));
            front.Z = (float)Math.Sin(MathHelper.DegreesToRadians(Yaw)) * (float)Math.Cos(MathHelper.DegreesToRadians(Pitch));
            Front = Vector3.Normalize(front);
            // Also re-calculate the Right and Up vector
            Right = Vector3.Normalize(Vector3.Cross(Front, WorldUp));  // Normalize the vectors, because their length gets closer to 0 the more you look up or down which results in slower movement.
            Up = Vector3.Normalize(Vector3.Cross(Right, Front));
        }
    }

}
