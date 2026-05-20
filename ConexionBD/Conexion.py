from enum import Enum
import mysql.connector

class TipoMotor(Enum):
    MYSQL = "MYSQL"
    SQLSERVER = "SQLSERVER"

class TipoAutenticacion(Enum):
    CREDENCIALES = "CREDENCIALES"
    WINDOWS = "WINDOWS"

class Conexion:
    _motorActual = TipoMotor.MYSQL
    _autenticacion = TipoAutenticacion.CREDENCIALES
    _host = "localhost"
    _puertoInstancia = "3306"
    _baseDatos = "corporativo_pos"
    _usuario = "root"
    _contrasena = "2306"
    _instancia = None

    @staticmethod
    def getMotor():
        return Conexion._motorActual

    @staticmethod
    def configurar(motor, auth, h, pi, bd, u, pass_):
        Conexion._motorActual = motor
        Conexion._autenticacion = auth
        Conexion._host = h
        Conexion._puertoInstancia = pi
        Conexion._baseDatos = bd
        Conexion._usuario = u
        Conexion._contrasena = pass_
        Conexion._instancia = None

    @staticmethod
    def getConexion():
        try:
            if Conexion._instancia is None or not Conexion._instancia.is_connected():
                if Conexion._motorActual == TipoMotor.MYSQL:
                    Conexion._instancia = mysql.connector.connect(
                        host=Conexion._host,
                        port=Conexion._puertoInstancia,
                        database=Conexion._baseDatos,
                        user=Conexion._usuario,
                        password=Conexion._contrasena
                    )
                    print("Conexion con MySQL establecida")
                else:
                    print("SQL Server no implementado en este script")
                    pass
                Conexion.asegurarColumnas()
        except Exception as e:
            print(e)
        return Conexion._instancia

    @staticmethod
    def cerrar():
        try:
            if Conexion._instancia is not None and Conexion._instancia.is_connected():
                Conexion._instancia.close()
                print("Conexion cerrada")
        except Exception as e:
            print(e)

    @staticmethod
    def asegurarColumnas():
        try:
            if Conexion._instancia is not None and Conexion._instancia.is_connected():
                cursor = Conexion._instancia.cursor()
                if Conexion._motorActual == TipoMotor.MYSQL:
                    sql = "ALTER TABLE ventas ADD COLUMN descuento DECIMAL(10,2) DEFAULT 0 AFTER total"
                else:
                    sql = "ALTER TABLE ventas ADD descuento DECIMAL(10,2) DEFAULT 0"
                cursor.execute(sql)
                Conexion._instancia.commit()
                cursor.close()
                print("Columna 'descuento' asegurada en la base de datos.")
        except Exception:
            pass
