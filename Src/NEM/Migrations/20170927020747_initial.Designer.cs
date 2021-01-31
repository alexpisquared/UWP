using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using NEM.DataModel;

namespace NEM.Migrations
{
    [DbContext(typeof(LocalStorageContext))]
    [Migration("20170927020747_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("NEM.DataModel.Episode", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<TimeSpan>("Duration");

                    b.Property<string>("FeedId");

                    b.Property<bool>("IsDownloaded");

                    b.Property<string>("ItemThumbnail");

                    b.Property<string>("Key");

                    b.Property<string>("LocalFileName");

                    b.Property<DateTimeOffset>("PublishDate");

                    b.Property<string>("Subtitle");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("EpisodeCache");
                });

            modelBuilder.Entity("NEM.DataModel.EpisodePlaybackState", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("EpisodeKey");

                    b.Property<double>("ListenProgress");

                    b.HasKey("Id");

                    b.ToTable("PlaybackState");
                });

            modelBuilder.Entity("NEM.DataModel.Favorite", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("EpisodeId");

                    b.HasKey("Id");

                    b.ToTable("Favorites");
                });

            modelBuilder.Entity("NEM.DataModel.InkNote", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("EpisodeKey");

                    b.Property<bool>("HasInk");

                    b.Property<string>("NoteText");

                    b.Property<byte[]>("Thumbnail");

                    b.Property<double>("Time");

                    b.HasKey("Id");

                    b.ToTable("Memes");
                });

            modelBuilder.Entity("NEM.DataModel.InkNoteData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("ImageBytes");

                    b.Property<byte[]>("Ink");

                    b.Property<Guid>("InkMeme");

                    b.HasKey("Id");

                    b.ToTable("MemeData");
                });
        }
    }
}
