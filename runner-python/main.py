import pygame
import time
import wincls


class Runner:
    def __init__(self):
        self.f = open('.cache\\1.dat')
        self.fs, self.fw, self.fh, self.fps = map(float, self.f.readline().split(','))
        self.fs, self.fh, self.fw = int(self.fs), int(self.fh), int(self.fw)
        self.s, self.a, self.caf = 0, self.fs, 1000 / self.fps
        super().__init__()

    def __print_page__(self):
        for _ in range(self.fh):
            print(self.f.readline(), end='')

    @staticmethod
    def __playBgm__():
        pygame.mixer.init()
        pygame.mixer.music.load(".cache\\2.mp3")
        pygame.mixer.music.play()

    def __control_runner__(self):
        stime = time.time()
        while self.fs != 0:
            ftime = time.time()
            if 1000 * (ftime - stime) > (self.a - self.fs) * self.caf:
                self.__print_page__()
                wincls.reset_cursor()
                self.fs = self.fs - 1
            else:
                time.sleep(self.caf / 1000 - 0.005)

    def run(self):
        wincls.init(self.fw, self.fh)
        wincls.cls()
        self.__playBgm__()
        self.__control_runner__()
        wincls.cls()


if __name__ == '__main__':
    Runner().run()
