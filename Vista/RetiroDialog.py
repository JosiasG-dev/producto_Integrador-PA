from PyQt6.QtWidgets import QDialog, QVBoxLayout, QHBoxLayout, QLabel, QLineEdit, QPushButton, QMessageBox
from PyQt6.QtCore import Qt
from Vista.Estilos import Estilos

class RetiroDialog(QDialog):
    def __init__(self, parent, ctrl):
        super().__init__(parent)
        self.ctrl = ctrl
        self.setWindowTitle("Retiro de Efectivo")
        self.setModal(True)
        self.setFixedSize(360, 240)
        self.construirUI()

    def construirUI(self):
        layout = QVBoxLayout(self)
        layout.setContentsMargins(20, 20, 20, 20)

        lblConcepto = QLabel("Concepto del retiro:")
        self.txtConcepto = Estilos.campo()
        layout.addWidget(lblConcepto)
        layout.addWidget(self.txtConcepto)

        lblMonto = QLabel("Monto ($):")
        self.txtMonto = Estilos.campo()
        layout.addWidget(lblMonto)
        layout.addWidget(self.txtMonto)

        btnLayout = QHBoxLayout()
        btnLayout.addStretch()
        
        btnCancelar = QPushButton("Cancelar")
        btnCancelar.clicked.connect(self.reject)
        
        btnRetirar = Estilos.botonPeligro("Retirar")
        btnRetirar.clicked.connect(self.procesarRetiro)
        
        btnLayout.addWidget(btnCancelar)
        btnLayout.addWidget(btnRetirar)
        layout.addLayout(btnLayout)

    def procesarRetiro(self):
        concepto = self.txtConcepto.text().strip()
        monto_str = self.txtMonto.text().strip()
        
        if not concepto or not monto_str:
            QMessageBox.warning(self, "Aviso", "Completa los campos")
            return
            
        try:
            monto = float(monto_str)
            if monto <= 0:
                QMessageBox.warning(self, "Aviso", "El monto debe ser mayor a 0")
                return
            if monto > self.ctrl.getMontoCaja():
                QMessageBox.warning(self, "Error", "Monto en caja insuficiente")
                return
                
            self.ctrl.registrarRetiro(concepto, monto)
            self.accept()
        except ValueError:
            QMessageBox.warning(self, "Error", "Monto invalido")
