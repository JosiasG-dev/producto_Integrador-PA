import logging
import traceback
import os
from PyQt6.QtWidgets import QMessageBox

class ManejoErrores:
    RUTA_LOG = "corporativo_pos_errores.log"
    LOGGER = logging.getLogger("CorporativoPOS")
    LOGGER.setLevel(logging.DEBUG)
    LOGGER.propagate = False
    
    _manejador_configurado = False
    
    if not _manejador_configurado:
        try:
            handler = logging.FileHandler(RUTA_LOG, mode='a')
            formatter = logging.Formatter('[%(asctime)s] [%(levelname)s] %(message)s', datefmt='%d/%m/%Y %H:%M:%S')
            handler.setFormatter(formatter)
            LOGGER.addHandler(handler)
            _manejador_configurado = True
        except Exception as e:
            print("No se pudo inicializar el archivo de log:", e)

    @staticmethod
    def error(padre, titulo, mensaje, ex=None):
        if ex is None:
            ManejoErrores.registrar(logging.ERROR, f"{titulo}: {mensaje}")
            QMessageBox.critical(padre, titulo, mensaje)
        else:
            ManejoErrores.registrarError(f"{titulo}: {mensaje}", ex)
            QMessageBox.critical(padre, titulo, f"{mensaje}\n\nDetalle: {str(ex)}")

    @staticmethod
    def advertencia(padre, titulo, mensaje):
        ManejoErrores.registrar(logging.WARNING, f"{titulo}: {mensaje}")
        QMessageBox.warning(padre, titulo, mensaje)

    @staticmethod
    def info(padre, titulo, mensaje):
        QMessageBox.information(padre, titulo, mensaje)

    @staticmethod
    def confirmar(padre, titulo, mensaje):
        resp = QMessageBox.question(padre, titulo, mensaje, QMessageBox.StandardButton.Yes | QMessageBox.StandardButton.No)
        return resp == QMessageBox.StandardButton.Yes

    @staticmethod
    def registrar(nivel, mensaje):
        ManejoErrores.LOGGER.log(nivel, mensaje)

    @staticmethod
    def registrarError(contexto, ex):
        tb = traceback.format_exc()
        ManejoErrores.LOGGER.error(f"{contexto}\n{tb}")

    @staticmethod
    def registrarInfo(evento):
        ManejoErrores.LOGGER.info(evento)

    @staticmethod
    def validarRequerido(padre, valor, nombreCampo):
        if valor is None or str(valor).strip() == "":
            ManejoErrores.advertencia(padre, "Campo requerido", f"El campo '{nombreCampo}' es obligatorio.")
            return False
        return True

    @staticmethod
    def validarNumero(padre, valor, nombreCampo):
        try:
            d = float(str(valor).strip().replace(",", "."))
            if d < 0:
                ManejoErrores.advertencia(padre, "Valor invalido", f"'{nombreCampo}' debe ser un numero positivo.")
                return -1.0
            return d
        except ValueError:
            ManejoErrores.advertencia(padre, "Formato incorrecto", f"'{nombreCampo}' debe ser un numero valido.")
            return -1.0

    @staticmethod
    def getRuta():
        return os.path.abspath(ManejoErrores.RUTA_LOG)
