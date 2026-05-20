using Microsoft.Extensions.DependencyInjection;

namespace punto_de_venta_C_;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		UserAppTheme = AppTheme.Light;
		MainThread.BeginInvokeOnMainThread(IniciarApp);
	}

	private async void IniciarApp()
	{
		var dlg = new Vista.ConexionDialog();
		bool conectado = await dlg.mostrar();
		if (conectado)
		{
			var ctrl = new Controlador.ControladorPrincipal();
			ctrl.iniciar();
		}
		else
		{
			Environment.Exit(0);
		}
	}
}