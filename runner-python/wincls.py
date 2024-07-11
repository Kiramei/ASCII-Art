import ctypes
import os


class COORD(ctypes.Structure):
    _fields_ = [("X", ctypes.c_short), ("Y", ctypes.c_short)]


def reset_cursor():
    STD_OUTPUT_HANDLE = -11
    std_out_handle = ctypes.windll.kernel32.GetStdHandle(STD_OUTPUT_HANDLE)
    dwCursorPosition = COORD()
    dwCursorPosition.X, dwCursorPosition.Y = 0, 0
    ctypes.windll.kernel32.SetConsoleCursorPosition(std_out_handle, dwCursorPosition)
    ctypes.windll.kernel32.SetConsoleCursorPosition(std_out_handle, dwCursorPosition)


def cls():
    os.system(r'cls')


def init(w: int, h: int) -> None:
    os.system(f'mode con cols={w} lines={h}')
