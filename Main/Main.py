import sys
import os

sys.path.append(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

from PyQt6.QtWidgets import QApplication
from ConexionBD.Conexion import Conexion
from Controlador.ControladorPrincipal import ControladorPrincipal
import atexit

def on_exit():
    Conexion.cerrar()

def main():
    app = QApplication(sys.argv)
    app.setStyle("Fusion")
    
    app.setStyleSheet("""
        QWidget {
            color: #18181B;
        }
        QDialog, QMainWindow, QFrame {
            background-color: #FAFAFA;
        }
        QTableWidget, QTableView {
            color: #18181B;
            gridline-color: #E4E4E7;
        }
        QHeaderView::section {
            color: #18181B;
            background-color: #F4F4F5;
        }
        QLineEdit, QComboBox, QSpinBox, QDoubleSpinBox, QTextEdit {
            color: #18181B;
        }
        QMessageBox {
            background-color: #FAFAFA;
        }
        QMessageBox QLabel {
            color: #18181B;
        }
    """)
    
    atexit.register(on_exit)

    from Vista.ConexionDialog import ConexionDialog
    dialogo = ConexionDialog()
    
    if not dialogo.exec():
        sys.exit(0)
        
    if not dialogo.fueConfirmado():
        sys.exit(0)

    controlador = ControladorPrincipal()
    controlador.iniciar()

    sys.exit(app.exec())

if __name__ == "__main__":
    main()
