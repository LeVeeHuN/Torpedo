# Torpedó
Ez egy kétszemélyes játék. A játéknak szüksége van egy szerverre, ez a Torpedo és két kliensre ez a TorpedoKliens. A kliens használja a szerver bizonyos elemeit, ezért egy project referencen át hivatkozik rá.
## A játék menete
A játék az ismert torpedón alapszik. Minden játékosnak a játék elején el kell helyeznie 5 hajót. A hajók 1-5 egység hosszúak lehetnek, egy hosszúságú hajóból csak egy tehető le. A hajókat nem lehet átlósan letenni, illetve két hajó nem érhet össze még sarokban sem.
## Hogyan játszható
### Helyi hálózaton belül
Lokális hálozaton belül nagyon egyszerű. Először meg kell tudnod a szervert futtató számítógép helyi IP-címét. Ezt több módón megteheted. Egy egyszerű példa rá:
1. Nyiss meg egy parncssort
2. Írd be, hogy `ipconfig`
3. Keresd meg azt a sort ami valahogy így néz ki: `IPv4 Address. . . . . . . . . . . : 192.168.1.65`
4. A sor végén látható IP-cím a lokális IP.
Miután ezzel megvagy, csak indítsd el a klienst két számítógépen és írd be ezt az IP-t, és mehet is a játék.
### Helyi hálózaton kívül, távolról
A játék távolról is játszahtó, viszont ilyenkor egy picivel több dolgot kell tenni mintha lokálisan játszanál. Az első lépés ugyanaz. Meg kell szerezned a szervert futtató számítógép lokális IP-címét, ezután a **router**-ed beállításaiban **port forwarding**-ot kell végrehajtanod. Az **_5100_**-as portot kell továbbítanod az előbb megszerzett lokális IP-re. Ha szükséges és meg kell adnod a protokollt, akkor TCP-t adj meg. Ezután, hogy távolról tudj csatlakozni a másik félnek a publikus IP-címedet kell használnia. Ezt megtudhatod pl. a googleből, ha rákeresel, hogy _what is my ip_.
