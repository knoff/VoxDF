# VoxDF
Indie 3D Voxel port of some aspects from Dwarf Fortress.


## Git helpers
http://stackoverflow.com/questions/18225126/how-to-use-git-for-unity3d-source-control
http://nvie.com/posts/a-successful-git-branching-model/
http://forum.unity3d.com/threads/unity-and-git.141420/


## Компоненты системы
### Entityloader (Загрузчик ресурсов)
Менеджер и загрузчик ресурсов предназначен для организации работы с динамически
загружаемыми ресурсами.
В качестве таких ресурсов могут выступать Игровые объекты (GameObject),
материалы (Material), текстуры. В дальнейшем будут добавлены и другие типы ресурсов.

Все ресурсы описываются конфигурационными файлами в формате yaml.

Основная задача менеджера - обеспечить возможность быстрого и простого использования
объектов, созданных на основе таких конфигурационных файлов.

Каждый загруженный в систему ресурс сохраняется в словаре и мтановится доступен по
своему имени. Имя ресурса состоит из пути и имени файла относительно корневой
папки с файлами ресурсов (расширение .yaml опускается), например ***Blocks/Core***.
Отдельно стоит обратить внимание на то, что независимо от особенностей файловой системы,
в качестве разделителя выступает символ "/".
#### Доступные методы:
+ ***void*** *LoadPrefab(**string** prefabName)* - Метод позволяет загрузить ресурс с указанным именем. Повторная загрузка ранее загруженного ресурса не производится.
+ ***void*** *LoadPrefab(**string** prefabName, **bool** force)* - Метод позволяет
загрузитьресурс с указанным именем. Если второй параметр задан как **true**,
будет произведена повторная загрузка ранее загруженного ресурса.
+ ***void*** *LoadPrefabFolder(**string** folderName)* - Метод позволяет загрузить
все ресурсы их указанной папки.

### CustomPrefab (Настраиваемый ресурс)
