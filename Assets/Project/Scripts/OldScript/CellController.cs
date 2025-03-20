//using system.collections.generic;
//using unityengine;
//using unityengine.ui;

//public class cellcontroller : monobehaviour
//{
//    [serializefield] private list<color> _availablecolors;
//    [serializefield] private transform _animationcanvas;

//    private list<int> _colors = new();

//    private list<cell> _cells = new();

//    private list<cellinfo> _allcellinfo = new();
//    private list<cellinfo> _horizontalcells = new();
//    private list<cellinfo> _verticalcells = new();

//    private cell _firstcell;
//    private cell _secondcell;

//    private list<int> _indexhorizontallinechek = new();
//    private list<int> _indexverticallinechek = new();

//    private list<cell> _cellsrecolor = new();
//    private list<image> destroyedimages = new();

//    private int _busyswapcount = 0;

//    public void init(int lineсount, int columncout)
//    {
//        for (int k = 0; k < _availablecolors.count; k++)
//            _colors.add(k);

//        for (int i = 0; i < columncout; i++)
//            _horizontalcells.add(new cellinfo());

//        for (int j = 0; j < lineсount; j++)
//            _verticalcells.add(new cellinfo());
//    }

//    public void addcell(cell cell)
//    {
//        _cells.add(cell);

//        _horizontalcells[cell.posy].cells.add(cell);
//        _verticalcells[cell.posx].cells.add(cell);

//        cell.setcolor(getrandomcolorwithoutrepetition(cell.posx, cell.posy), _availablecolors);
//    }

//    public void oncellclicked(cell cell)
//    {
//        if (_firstcell == cell || _busyswapcount > 0)
//        {
//            resetselection();
//            return;
//        }

//        if (_firstcell == null)
//            _firstcell = cell;
//        else if (_secondcell == null)
//        {
//            _secondcell = cell;
//            checkandswapcells();
//        }
//    }

//    private void checkandswapcells()
//    {
//        if (cellsisneighbors(_firstcell, _secondcell))
//        {
//            if (havematch())
//                swapcells(_firstcell, _secondcell);
//            else
//                dofakeanimation();
//        }
//    }

//    private bool cellsisneighbors(cell cell1, cell cell2)
//    {
//        return mathf.abs(cell1.posx - cell2.posx) + mathf.abs(cell1.posy - cell2.posy) == 1;
//    }

//    private void swapcells(cell cell1, cell cell2)
//    {
//        var tempimage = cell1.figureimage;
//        var tempcolor = cell1.colornumber;

//        _busyswapcount += 2;

//        cell1.figureimage = cell2.figureimage;
//        cell1.setcolor(cell2.colornumber, _availablecolors);
//        cell1.movefigureimagetooriginalposition(0.25f, false);

//        cell2.figureimage = tempimage;
//        cell2.setcolor(tempcolor, _availablecolors);
//        cell2.movefigureimagetooriginalposition(0.25f, true);

//        resetselection();  // сбрасываем выбор
//    }

//    public void resetselection()
//    {
//        _firstcell = null;
//        _secondcell = null;
//    }

//    private void dofakeanimation()
//    {
//        _busyswapcount += 2;
//        _firstcell.fakeswapanimation(_secondcell.getanchoredimageposition(), 0.25f);
//        _secondcell.fakeswapanimation(_firstcell.getanchoredimageposition(), 0.25f);
//    }
//    public void onmovecomplete(bool searchmatch)
//    {
//        --_busyswapcount;
//        if (searchmatch)
//            searchcelltomatch();
//    }

//    public void removecellsparent()
//    {
//        foreach (var cell in _cells)
//            cell.setparentforimage(_animationcanvas);
//    }

//    private bool havematch()
//    {
//        bool match = false;

//        if (_firstcell.colornumber == _secondcell.colornumber) return false;

//        _indexhorizontallinechek.clear();
//        _indexverticallinechek.clear();

//        if (_firstcell.posx == _secondcell.posx)  //verticalswipe
//        {
//            _indexhorizontallinechek.add(_firstcell.posy);
//            _indexhorizontallinechek.add(_secondcell.posy);
//            _indexverticallinechek.add(_firstcell.posx);
//        }
//        else
//        {
//            _indexhorizontallinechek.add(_firstcell.posy);
//            _indexverticallinechek.add(_firstcell.posx);
//            _indexverticallinechek.add(_secondcell.posx);
//        }

//        match = smartcheck();

//        return match;
//    }

//    private bool smartcheck()
//    {
//        bool havematch = false;

//        foreach (var index in _indexhorizontallinechek)
//        {
//            havematch = checkforthreeinarow(_horizontalcells[index].cells);
//            if (havematch) return true;
//        }

//        foreach (var index in _indexverticallinechek)
//        {
//            havematch = checkforthreeinarow(_verticalcells[index].cells);
//            if (havematch) return true;
//        }

//        return havematch;
//    }

//    private bool checkforthreeinarow(list<cell> cells)
//    {
//        _cellsrecolor = new list<cell>(cells);

//        находим индексы элементов
//        int indexfirstcell = _cellsrecolor.indexof(_firstcell);
//        int indexsecondcell = _cellsrecolor.indexof(_secondcell);

//        если первый элемент найден, а второй нет, замен€ем первый на второй(если второй существует)
//        if (indexfirstcell != -1 && indexsecondcell == -1)
//            _cellsrecolor[indexfirstcell] = _secondcell;

//        если второй элемент найден, а первый нет, замен€ем второй на первый
//        else if (indexsecondcell != -1 && indexfirstcell == -1)
//            _cellsrecolor[indexsecondcell] = _firstcell;

//        если оба элемента найдены, мен€ем их местами
//        else if (indexfirstcell != -1 && indexsecondcell != -1)
//        {
//            cell temp = _cellsrecolor[indexfirstcell];
//            _cellsrecolor[indexfirstcell] = _cellsrecolor[indexsecondcell];
//            _cellsrecolor[indexsecondcell] = temp;
//        }

//        for (int i = 0; i <= _cellsrecolor.count - 3; i++)
//        {
//            if (_cellsrecolor[i].colornumber == _cellsrecolor[i + 1].colornumber &&
//                _cellsrecolor[i].colornumber == _cellsrecolor[i + 2].colornumber)
//                return true;
//        }

//        return false;
//    }

//    public void searchcelltomatch()
//    {
//        if (_busyswapcount == 0)
//        {
//            if (_allcellinfo.count == 0)
//            {
//                _allcellinfo.addrange(_horizontalcells);
//                _allcellinfo.addrange(_verticalcells);
//            }

//            int matchcount;
//            int lenghtcount;

//            foreach (var line in _allcellinfo)
//            {
//                matchcount = 1;
//                lenghtcount = line.cells.count - 1;

//                for (int i = 1; i < line.cells.count; i++)
//                {
//                    if (line.cells[i].colornumber == line.cells[i - 1].colornumber)
//                        matchcount++;
//                    else
//                    {
//                        если найдено 3 или более одинаковых элементов подр€д
//                        if (matchcount >= 3)
//                        {
//                            вызываем puff() дл€ всех элементов в последовательности
//                            for (int j = i - 1; j >= i - matchcount; j--)
//                                line.cells[j].puff();
//                        }

//                        matchcount = 1;
//                    }

//                    если последовательность закончилась последним элементом
//                    if (i == lenghtcount && matchcount >= 3)
//                    {
//                        for (int j = lenghtcount; j > lenghtcount - matchcount; j--)
//                            line.cells[j].puff();
//                    }
//                }
//            }

//            cellmovedown();
//        }
//    }

//    private void cellmovedown()
//    {
//        foreach (var line in _verticalcells)
//        {
//            destroyedimages.clear();

//            проходим по €чейкам снизу вверх
//            for (int i = line.cells.count - 1; i >= 0; i--)
//            {
//                if (line.cells[i].haspuffed)
//                {
//                    destroyedimages.add(line.cells[i].figureimage);
//                    line.cells[i].haspuffed = false;
//                }
//                else if (destroyedimages.count > 0)
//                {
//                    var targecell = line.cells[i + destroyedimages.count];

//                    targecell.figureimage = line.cells[i].figureimage;
//                    targecell.colornumber = line.cells[i].colornumber;
//                    ++_busyswapcount;
//                    targecell.movefigureimagetooriginalposition(0.8f, true);
//                }
//            }

//            if (destroyedimages.count > 0)
//            {
//                var j = destroyedimages.count - 1;

//                foreach (var cell in line.cells)
//                {
//                    cell.figureimage = destroyedimages[j];
//                    cell.setrandomcolor(_availablecolors);
//                    cell.setimagefigureposition(line.cells[0].getoriginalposition() + new vector2(0, (j + 1) * cell.rectranform.sizedelta.y));
//                    ++_busyswapcount;
//                    cell.movefigureimagetooriginalposition(0.8f, true);

//                    if (--j < 0) break;
//                }
//            }
//        }
//    }

//    private int getrandomcolorwithoutrepetition(int x, int y)
//    {
//        var countignore = 0;

//        if (x >= 2)
//        {
//            int left1 = getcolorbycoordinate(x - 1, y);

//            if (left1 == getcolorbycoordinate(x - 2, y))
//            {
//                ++countignore;
//                movetoend(left1);
//            }
//        }

//        проверка вертикальной линии(2 предыдущих элемента)
//        if (y >= 2)
//        {
//            int up1 = getcolorbycoordinate(x, y - 1);

//            if (up1 == getcolorbycoordinate(x, y - 2))
//            {
//                ++countignore;
//                movetoend(up1);
//            }
//        }

//        int randomindex = random.range(0, _colors.count - countignore);

//        return _colors[randomindex];
//    }

//    private void movetoend(int colornumber)
//    {
//        _colors.remove(colornumber);
//        _colors.add(colornumber);
//    }

//    private int getcolorbycoordinate(int x, int y)
//    {
//        int color = 0;

//        foreach (var cell in _cells)
//        {
//            if (cell.posx == x && cell.posy == y)
//            {
//                color = cell.colornumber;
//                break;
//            }
//        }

//        return color;
//    }
//}

//[system.serializable]
//public class cellinfo
//{
//    public list<cell> cells = new();
//}