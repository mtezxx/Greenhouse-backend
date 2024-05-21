#!/bin/sh
if [ ! -f $DBDIR/Greenhouse.db ] ; then
  cp /app/Greenhouse2.db $DBDIR/Greenhouse.db
fi
dotnet WebAPI.dll