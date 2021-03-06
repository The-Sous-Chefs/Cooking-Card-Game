# Unnamed, But Brilliant, Cooking Card Game

## Description

A card game that takes place in a procedurally-generated, food themed 3D world. Currently being developed by Diyuan Dai, Irena Lee, Ian Thorne, Mindy Van, and Yaswin Veluguleti, with the help of Dr. Sarah Abraham.

## For Developers

Since 3D game development can include a lot of large binary files, this repo is set up to use git lfs to keep the repo size down. In a nutshell, git lfs will replace some files with pointers to files stored in other locations, thus preventing large textures, models, etc. from being stored in the repo itself. On your local machine, however, there shouldn't really be any difference, as though the files themselves were there.

**Before you clone this repo** you should download and install git lfs! You can download git lfs [here](https://git-lfs.github.com/), but if you're using a Mac, you can also download it with Homebrew with `brew install git-lfs` (Isn't Homebrew nice?). Once you've got it installed, you can simply run `git lfs install` and it will be set up for your user account on your machine. It's basically magic, to me.

git lfs knows which files to track differently based on the `.gitattributes` file in the top-most folder of this repo. If we ever find that we need to have git lfs track other files, the command `git lfs track "*.[filetype]"` will have git lfs track all `.[filetype]` files. The `.gitattributes` file in this repo was copied from another Unity 3D project, however, so if we discover that some weird behaviors are going on, the documentation for the `.gitattributes` file can be found [here](https://git-scm.com/docs/gitattributes)!

One more thing: The `.gitignore` in this repo was also copied from another Unity 3D project, so it may need some work as well!
