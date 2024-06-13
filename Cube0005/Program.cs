using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;


public class Window : GameWindow
{
    private int _vertexBufferObject;
    private int _vertexArrayObject;
    private Shader _shader;
    private readonly float[] _vertices = {
    // Позиции          // Цвета
    // Передняя грань
    -0.5f, -0.5f,  0.5f,  1.0f, 0.0f, 0.0f,  // Нижняя левая (красный)
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f, 0.0f,  // Нижняя правая (красный)
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f, 0.0f,  // Верхняя правая (красный)
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f, 0.0f,  // Верхняя правая (красный)
    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f, 0.0f,  // Верхняя левая (красный)
    -0.5f, -0.5f,  0.5f,  1.0f, 0.0f, 0.0f,  // Нижняя левая (красный)

    // Задняя грань
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f, 0.0f,  // Нижняя левая (зеленый)
     0.5f, -0.5f, -0.5f,  0.0f, 1.0f, 0.0f,  // Нижняя правая (зеленый)
     0.5f,  0.5f, -0.5f,  0.0f, 1.0f, 0.0f,  // Верхняя правая (зеленый)
     0.5f,  0.5f, -0.5f,  0.0f, 1.0f, 0.0f,  // Верхняя правая (зеленый)
    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f, 0.0f,  // Верхняя левая (зеленый)
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f, 0.0f,  // Нижняя левая (зеленый)

    // Левая грань
    -0.5f,  0.5f,  0.5f,  0.0f, 0.0f, 1.0f,  // Верхняя левая (синий)
    -0.5f,  0.5f, -0.5f,  0.0f, 0.0f, 1.0f,  // Верхняя правая (синий)
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f, 1.0f,  // Нижняя правая (синий)
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f, 1.0f,  // Нижняя правая (синий)
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f, 1.0f,  // Нижняя левая (синий)
    -0.5f,  0.5f,  0.5f,  0.0f, 0.0f, 1.0f,  // Верхняя левая (синий)

    // Правая грань
     0.5f,  0.5f,  0.5f,  1.0f, 1.0f, 0.0f,  // Верхняя левая (желтый)
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f, 0.0f,  // Верхняя правая (желтый)
     0.5f, -0.5f, -0.5f,  1.0f, 1.0f, 0.0f,  // Нижняя правая (желтый)
     0.5f, -0.5f, -0.5f,  1.0f, 1.0f, 0.0f,  // Нижняя правая (желтый)
     0.5f, -0.5f,  0.5f,  1.0f, 1.0f, 0.0f,  // Нижняя левая (желтый)
     0.5f,  0.5f,  0.5f,  1.0f, 1.0f, 0.0f,  // Верхняя левая (желтый)

    // Верхняя грань
    -0.5f,  0.5f, -0.5f,  1.0f, 0.0f, 1.0f,  // Верхняя левая (фиолетовый)
     0.5f,  0.5f, -0.5f,  1.0f, 0.0f, 1.0f,  // Верхняя правая (фиолетовый)
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f, 1.0f,  // Нижняя правая (фиолетовый)
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f, 1.0f,  // Нижняя правая (фиолетовый)
    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f, 1.0f,  // Нижняя левая (фиолетовый)
    -0.5f,  0.5f, -0.5f,  1.0f, 0.0f, 1.0f,  // Верхняя левая (фиолетовый)

    // Нижняя грань
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f, 1.0f,  // Верхняя левая (голубой)
     0.5f, -0.5f, -0.5f,  0.0f, 1.0f, 1.0f,  // Верхняя правая (голубой)
     0.5f, -0.5f,  0.5f,  0.0f, 1.0f, 1.0f,  // Нижняя правая (голубой)
     0.5f, -0.5f,  0.5f,  0.0f, 1.0f, 1.0f,  // Нижняя правая (голубой)
    -0.5f, -0.5f,  0.5f,  0.0f, 1.0f, 1.0f,  // Нижняя левая (голубой)
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f, 1.0f   // Верхняя левая (голубой)
};


    private Matrix4 _model;
    private Matrix4 _projection;
    private Camera _camera;

    private Vector2 _lastMousePosition;
    private bool _firstMove = true;

    public Window(int width, int height, string title)
        : base(width, height, GraphicsMode.Default, title)
    {
        _camera = new Camera(Vector3.UnitZ * 3, Vector3.UnitZ * -1, Vector3.UnitY);
        CursorVisible = false; // Скрыть курсор
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Установка цвета фона
        GL.ClearColor(0.1f, 0.2f, 0.3f, 1.0f);

        // Генерация и привязка буфера вершин
        _vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

        // Генерация и привязка вершинного массива
        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);

        // Указание атрибута вершин
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        // Указание атрибута цвета
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        // Загрузка и компиляция шейдеров
        _shader = new Shader("vertex_shader.glsl", "fragment_shader.glsl");
        _shader.Use();

        // Установка матриц
        _model = Matrix4.Identity;
        _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Width / (float)Height, 0.1f, 100.0f);

        // Передача матриц в шейдеры
        int modelLocation = GL.GetUniformLocation(_shader.Handle, "model");
        GL.UniformMatrix4(modelLocation, false, ref _model);

        int projectionLocation = GL.GetUniformLocation(_shader.Handle, "projection");
        GL.UniformMatrix4(projectionLocation, false, ref _projection);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        // Включение теста глубины
        GL.Enable(EnableCap.DepthTest);

        // Очистка экрана (включая буфер глубины)
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // Использование шейдера
        _shader.Use();

        // Обновление матрицы вида
        Matrix4 view = _camera.GetViewMatrix();
        int viewLocation = GL.GetUniformLocation(_shader.Handle, "view");
        GL.UniformMatrix4(viewLocation, false, ref view);

        // Привязка вершинного массива и рендеринг
        GL.BindVertexArray(_vertexArrayObject);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

        // Swap buffers
        Context.SwapBuffers();
    }


    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        var keyboardState = Keyboard.GetState();

        // Обработка ввода с клавиатуры
        if (keyboardState.IsKeyDown(Key.W))
        {
            _camera.ProcessKeyboard((float)e.Time, forward: true, backward: false, left: false, right: false);
        }
        if (keyboardState.IsKeyDown(Key.S))
        {
            _camera.ProcessKeyboard((float)e.Time, forward: false, backward: true, left: false, right: false);
        }
        if (keyboardState.IsKeyDown(Key.A))
        {
            _camera.ProcessKeyboard((float)e.Time, forward: false, backward: false, left: true, right: false);
        }
        if (keyboardState.IsKeyDown(Key.D))
        {
            _camera.ProcessKeyboard((float)e.Time, forward: false, backward: false, left: false, right: true);
        }
        if (keyboardState.IsKeyDown(Key.Escape))
        {
            this.Close();
        }

        // Обработка ввода с мыши
        var mouseState = Mouse.GetState();
        var mousePosition = new Vector2(mouseState.X, mouseState.Y);

        if (_firstMove) // Если это первое движение мыши, просто установите последнее положение мыши на текущее
        {
            _lastMousePosition = mousePosition;
            _firstMove = false;
        }
        else
        {
            var deltaX = mousePosition.X - _lastMousePosition.X;
            var deltaY = mousePosition.Y - _lastMousePosition.Y;
            _lastMousePosition = mousePosition;

            _camera.ProcessMouseMovement(deltaX, deltaY);
        }
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, Width, Height);

        _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Width / (float)Height, 0.1f, 100.0f);
        int projectionLocation = GL.GetUniformLocation(_shader.Handle, "projection");
        GL.UniformMatrix4(projectionLocation, false, ref _projection);
    }

    protected override void OnUnload(EventArgs e)
    {
        base.OnUnload(e);

        // Очистка ресурсов
        GL.DeleteBuffer(_vertexBufferObject);
        GL.DeleteVertexArray(_vertexArrayObject);
        _shader.Dispose();
    }

    [STAThread]
    public static void Main()
    {
        using (var window = new Window(800, 600, "OpenTK Example"))
        {
            window.Run(60.0);
        }
    }
}

// Класс для работы с шейдерами
public class Shader
{
    public int Handle;

    public Shader(string vertexPath, string fragmentPath)
    {
        // Загрузка кода шейдера
        string vertexShaderSource = System.IO.File.ReadAllText(vertexPath);
        string fragmentShaderSource = System.IO.File.ReadAllText(fragmentPath);

        // Создание и компиляция вершинного шейдера
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);
        GL.CompileShader(vertexShader);
        CheckShaderCompileStatus(vertexShader);

        // Создание и компиляция фрагментного шейдера
        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        GL.CompileShader(fragmentShader);
        CheckShaderCompileStatus(fragmentShader);

        // Создание программы шейдера и линковка
        Handle = GL.CreateProgram();
        GL.AttachShader(Handle, vertexShader);
        GL.AttachShader(Handle, fragmentShader);
        GL.LinkProgram(Handle);
        CheckProgramLinkStatus(Handle);

        // Очистка
        GL.DetachShader(Handle, vertexShader);
        GL.DetachShader(Handle, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    public void Use()
    {
        GL.UseProgram(Handle);
    }

    public void Dispose()
    {
        GL.DeleteProgram(Handle);
    }

    private void CheckShaderCompileStatus(int shader)
    {
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetShaderInfoLog(shader);
            throw new Exception($"Ошибка компиляции шейдера: {infoLog}");
        }
    }

    private void CheckProgramLinkStatus(int program)
    {
        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetProgramInfoLog(program);
            throw new Exception($"Ошибка линковки программы: {infoLog}");
        }
    }
}

// Класс камеры
public class Camera
{
    private Vector3 _position;
    private Vector3 _front;
    private Vector3 _up;
    private float _pitch;
    private float _yaw;
    private float _speed;

    public Camera(Vector3 position, Vector3 front, Vector3 up, float speed = 1.5f)
    {
        _position = position;
        _front = front;
        _up = up;
        _pitch = 0.0f;
        _yaw = -90.0f;
        _speed = speed;
    }

    public Matrix4 GetViewMatrix()
    {
        return Matrix4.LookAt(_position, _position + _front, _up);
    }

    public void ProcessKeyboard(float deltaTime, bool forward, bool backward, bool left, bool right)
    {
        float velocity = _speed * deltaTime;

        if (forward)
            _position += _front * velocity;
        if (backward)
            _position -= _front * velocity;
        if (left)
            _position -= Vector3.Normalize(Vector3.Cross(_front, _up)) * velocity;
        if (right)
            _position += Vector3.Normalize(Vector3.Cross(_front, _up)) * velocity;
    }

    public void ProcessMouseMovement(float xOffset, float yOffset, bool constrainPitch = true)
    {
        float sensitivity = 0.1f;
        xOffset *= sensitivity;
        yOffset *= sensitivity;

        _yaw += xOffset;
        _pitch -= yOffset;

        if (constrainPitch)
        {
            if (_pitch > 89.0f)
                _pitch = 89.0f;
            if (_pitch < -89.0f)
                _pitch = -89.0f;
        }

        UpdateCameraVectors();
    }

    private void UpdateCameraVectors()
    {
        Vector3 front;
        front.X = (float)Math.Cos(MathHelper.DegreesToRadians(_yaw)) * (float)Math.Cos(MathHelper.DegreesToRadians(_pitch));
        front.Y = (float)Math.Sin(MathHelper.DegreesToRadians(_pitch));
        front.Z = (float)Math.Sin(MathHelper.DegreesToRadians(_yaw)) * (float)Math.Cos(MathHelper.DegreesToRadians(_pitch));
        _front = Vector3.Normalize(front);
    }
}
