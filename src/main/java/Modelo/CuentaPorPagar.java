package Modelo;

public class CuentaPorPagar {
	public enum Estado {
		PENDIENTE, PARCIAL, PAGADO
	}

	private int id;
	private Proveedor proveedor;
	private int ordenId;
	private double montoTotal;
	private double pagado;
	private double saldo;
	private String vencimiento;
	private Estado estado;

	public CuentaPorPagar(int id, Proveedor proveedor, int ordenId, double montoTotal, double pagado, double saldo,
			String vencimiento, Estado estado) {
		this.id = id;
		this.proveedor = proveedor;
		this.ordenId = ordenId;
		this.montoTotal = montoTotal;
		this.pagado = pagado;
		this.saldo = saldo;
		this.vencimiento = vencimiento;
		this.estado = estado;
	}

	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public Proveedor getProveedor() {
		return proveedor;
	}

	public void setProveedor(Proveedor proveedor) {
		this.proveedor = proveedor;
	}

	public int getOrdenId() {
		return ordenId;
	}

	public void setOrdenId(int ordenId) {
		this.ordenId = ordenId;
	}

	public double getMontoTotal() {
		return montoTotal;
	}

	public void setMontoTotal(double montoTotal) {
		this.montoTotal = montoTotal;
	}

	public double getPagado() {
		return pagado;
	}

	public void setPagado(double pagado) {
		this.pagado = pagado;
	}

	public double getSaldo() {
		return saldo;
	}

	public void setSaldo(double saldo) {
		this.saldo = saldo;
	}

	public String getVencimiento() {
		return vencimiento;
	}

	public void setVencimiento(String vencimiento) {
		this.vencimiento = vencimiento;
	}

	public Estado getEstado() {
		return estado;
	}

	public void setEstado(Estado estado) {
		this.estado = estado;
	}

	public void aplicarPago(double monto) {
		this.pagado += monto;
		this.saldo = this.montoTotal - this.pagado;
		if (this.saldo <= 0) {
			this.saldo = 0;
			this.estado = Estado.PAGADO;
		} else {
			this.estado = Estado.PARCIAL;
		}
	}

	public String getFolioOrden() {
		return "OC-" + String.format("%04d", ordenId % 10000);
	}
}
