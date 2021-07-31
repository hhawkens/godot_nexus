namespace App.Shell.Addons

open System

type internal IGodotVersionQuery =
    abstract IsVersion: string -> bool
    abstract GetArchiveVersion: string -> Version option
    abstract GetMonoArchiveVersion: string -> Version option
