using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mode7.CameraEngine
{
    public class Mode7Camera
    {
        Viewport _viewport;

        public Mode7Camera(
            Viewport viewport,
            Vector2 position,
            float rotation = 0,
            float zoom = 1)
        {
            _viewport = viewport;
            Position = position;
            Rotation = rotation;
            Zoom = zoom;
        }

        public Matrix WorldToScreen()
        {
            var viewportCentre = new Vector2(_viewport.Width / 2, _viewport.Height / 2);

            var matrix = Matrix.CreateTranslation(new Vector3(-Position, 0))
                * Matrix.CreateRotationZ(Rotation)
                * Matrix.CreateScale(new Vector3(Zoom, Zoom, 1))
                * Matrix.CreateTranslation(new Vector3(viewportCentre, 0));

            matrix *= ComputeMatrix(
                new Vector2(_viewport.Width, _viewport.Height),
                new Vector2(0, _viewport.Height / 2),
                new Vector2(_viewport.Width, _viewport.Height / 2),
                new Vector2(-_viewport.Width, _viewport.Height),
                new Vector2(_viewport.Width*2, _viewport.Height));

            return matrix;
        }

        public Matrix ScreenToWorld()
        {
            return Matrix.Invert(WorldToScreen());
        }

        public Vector2 Position { get; set; }

        public float Rotation { get; set; }

        public float Zoom { get; set; }

        static Matrix ComputeAffineTransform(Vector2 ptUL, Vector2 ptUR, Vector2 ptLL)
        {
            return new Matrix()
            {
                M11 = (ptUR.X - ptUL.X),
                M12 = (ptUR.Y - ptUL.Y),
                M21 = (ptLL.X - ptUL.X),
                M22 = (ptLL.Y - ptUL.Y),
                M33 = 1,
                M41 = ptUL.X,
                M42 = ptUL.Y,
                M44 = 1
            };
        }

        static Matrix ComputeMatrix(Vector2 size, Vector2 ptUL, Vector2 ptUR, Vector2 ptLL, Vector2 ptLR)
        {
            // Scale transform
            Matrix S = Matrix.CreateScale(1 / size.X, 1 / size.Y, 1);
            // Affine transform
            Matrix A = ComputeAffineTransform(ptUL, ptUR, ptLL);

            // Non-Affine transform
            Matrix B = new Matrix();
            float den = A.M11 * A.M22 - A.M12 * A.M21;
            float a = (A.M22 * ptLR.X - A.M21 * ptLR.Y +
                       A.M21 * A.M42 - A.M22 * A.M41) / den;
            float b = (A.M11 * ptLR.Y - A.M12 * ptLR.X +
                       A.M12 * A.M41 - A.M11 * A.M42) / den;
            B.M11 = a / (a + b - 1);
            B.M22 = b / (a + b - 1);
            B.M33 = 1;
            B.M14 = B.M11 - 1;
            B.M24 = B.M22 - 1;
            B.M44 = 1;
            // Product of three transforms
            return S * B * A;
        }
    }
}
