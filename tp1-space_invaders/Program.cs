namespace tp1_space_invaders
{
    public class Program
    {
        public static void Main()
        {
            var juego = new Juego();

        }
    }

    public class Juego
    {
        const int MAPA_ANCHO = 15;
        const int MAPA_ALTO = 10;
        const int NUM_ENEMIGOS = 6;

        int frame;
        Viewport viewport;
        Personaje jugador;
        List<Personaje> enemigos;
        List<Proyectil> misiles;
        Habitacion habitacion;
        


        public Juego()
        {

            frame = 0;
            viewport = new Viewport(MAPA_ANCHO, MAPA_ALTO);
            jugador = new Personaje(8, 7, '^');
            habitacion = new Habitacion(MAPA_ANCHO, MAPA_ALTO);
            misiles = new List<Proyectil>();
            enemigos = new List<Personaje>();

            int filaBase = 2;
            int y = filaBase;
            int distanciaEntreEnemigos = 1; 
            int posicionX = 2;
            int enemigosPorFila = 0;  


        
            for (int i = 0; i < NUM_ENEMIGOS; i++)
            {  
                enemigos.Add(new Personaje(posicionX, y, '='));

                enemigosPorFila++;
                if (enemigosPorFila >= 3) 
                {
                    
                    enemigosPorFila = 0;
                    y++; 
                }

                posicionX += 1 + distanciaEntreEnemigos; 
            }

            GameLoop();
        }

        private void GameLoop()
        {
            while (true)
            {
                
                ConsoleKeyInfo? input = null;
                if (Console.KeyAvailable)
                    input = Console.ReadKey(true);

                
                ActualizarDatos(input);

                
                DibujarPantalla(); 

                System.Threading.Thread.Sleep(300);
            }
        }

        private void ActualizarDatos(ConsoleKeyInfo? input)
        {
            frame++;

            if (input.HasValue)
            {
                switch (input.Value.Key)
                {
                    case ConsoleKey.LeftArrow:

                        if (jugador.x > 1) 
                            jugador.MoverHacia(-1, 0);
                        break;

                    case ConsoleKey.RightArrow:
                        if (jugador.x < MAPA_ANCHO -2)     
                            jugador.MoverHacia(1, 0);
                        break;
                        
                    case ConsoleKey.Spacebar:
                        misiles.Add(new Proyectil(jugador.x, jugador.y, '|')); 
                        break;
                }

            }

            
            foreach (var misil in misiles.ToList())
            {
                misil.MoverHacia(0, -1);

                if (misil.y <= 0) 
                {
                    misiles.Remove(misil);
                }

                foreach (var enemigo in enemigos.ToList())
                {
                    if (misil.x == enemigo.x && misil.y == enemigo.y)
                    {
                        misiles.Remove(misil);
                        enemigos.Remove(enemigo);
                    }
                }
            }

        }

        private void DibujarPantalla()
        {
            viewport.LimpiarPantalla();
            habitacion.Dibujar(viewport);
            jugador.Dibujar(viewport);  

            foreach (var misil in misiles)
            {
                misil.Dibujar(viewport);
            }

            foreach (var enemigo in enemigos)
            {
                enemigo.Dibujar(viewport);
            }
            

            viewport.MostrarEnPantalla(); 
            Console.WriteLine($"Frame: {frame}");
        }
    }

    class Personaje
    {

        public int x, y;
        private char dibujo = '^';


        public Personaje(int x, int y, char dibujo)
        {
            this.x = x;
            this.y = y;
            this.dibujo = dibujo;  
        }

        public void MoverHacia(int x, int y)
        {
            this.x += x;
            this.y += y;
        }

        public void Dibujar(Viewport viewport)
        {
            viewport.Dibujar(x, y, dibujo);
        }
    }


    class Viewport
    {
        private char[,] celdas;
        private int ancho, alto;

        public Viewport(int ancho, int alto)
        {
            this.ancho = ancho;
            this.alto = alto;
            celdas = new char[ancho, alto];
        }

        public void Dibujar(int x, int y, char celda)
        {
            celdas[x, y] = celda;
        }

        public void MostrarEnPantalla()
        {
        
            Console.Clear();

            for (int y = 0; y < alto; y++)
            {
                for (int x = 0; x < ancho; x++)
                {
                    Console.Write(celdas[x, y]);
                }
                Console.Write("\n");
            }
        }

        internal void LimpiarPantalla()
        {
            for (int y = 0; y < alto; y++)
            {
                for (int x = 0; x < ancho; x++)
                {   
                    celdas[x, y] = ' ';
                }
            }
        }
    }

    class Proyectil
    {
        public int x, y;

        private char dibujo = '|';

        public Proyectil(int x, int y, char dibujo)
        {
            this.x = x;
            this.y = y;
            this.dibujo = dibujo;
        }

        public void MoverHacia(int x, int y)
        {
            this.x += x;
            this.y += y;
        }

        public void Dibujar(Viewport viewport)
        {
            viewport.Dibujar(x, y, dibujo);
        }
    }

    interface IMapa
    {
        bool EstaLibre(int x, int y);      
    }

    class Habitacion : IMapa
    {
        private List<Fila> filas;

        public Habitacion(int ancho, int alto)
        {
            filas = new List<Fila>();

            filas.Add(new FilaBorde(ancho));
            for (int fila = 1; fila < alto - 1; fila++)
            {
                filas.Add(new FilaMedia(ancho));
            }
            filas.Add(new FilaBorde(ancho));
        }

        public void Dibujar(Viewport viewport)
        {
            for (int y = 0; y < filas.Count(); y++)
            {
                filas[y].Dibujar(viewport, y);
            }
        }

        public bool EstaLibre(int x, int y)
        {
            return filas[y].EstaLibre(x);      
        }
    }

    abstract class Fila
    {
        protected List<char> celdas;

        public Fila(int cantidadCeldas)
        {
            this.celdas = new List<char>();

            AgregarPunta();

            for (int i = 1; i < cantidadCeldas - 1; i++)
            {
                AgregarMedio();
            }

            AgregarPunta();
        }

        protected abstract void AgregarMedio();
        protected abstract void AgregarPunta();

        public void Dibujar(Viewport viewport, int y)
        {
            for (int x = 0; x < celdas.Count(); x++)
            {
                viewport.Dibujar(x, y, celdas[x]);
            }
        }

        internal bool EstaLibre(int x)
        {
            return celdas[x] == ' ';
        }
    }

    class FilaMedia : Fila
    {
        public FilaMedia(int cantidadCeldas) : base(cantidadCeldas)
        {
        }

        protected override void AgregarMedio()
        {
            celdas.Add(' ');
        }
        protected override void AgregarPunta()
        {
            celdas.Add('#');
        }
    }

    class FilaBorde : Fila
    {
        public FilaBorde(int cantidadCeldas) : base(cantidadCeldas)
        {
        }

        protected override void AgregarMedio()
        {
            celdas.Add('#');
        }
        protected override void AgregarPunta()
        {
            celdas.Add('#');
        }
    }

}

