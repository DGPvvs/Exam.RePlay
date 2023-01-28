using System;
using System.Collections.Generic;
using System.Linq;

namespace Exam.RePlay
{
	public class RePlayer : IRePlayer
	{
		private HashSet<Track> traks = new HashSet<Track>();
		private Dictionary<String, Track> traksById = new Dictionary<string, Track>();
		private Dictionary<string, Dictionary<string, Track>> albums = new Dictionary<string, Dictionary<string, Track>>();
		private Queue<Track> listeningQueue = new Queue<Track>();

		public void AddTrack(Track track, string album)
		{
			if (!this.albums.ContainsKey(album))
			{
				this.albums.Add(album, new Dictionary<string, Track>());
			}

			if (!this.albums[album].ContainsKey(track.Title))
			{
				this.albums[album].Add(track.Title, track);
				track.Album = album;
			}

			this.traks.Add(track);
			this.traksById.Add(track.Id, track);
		}

		public bool Contains(Track track) => this.traksById.ContainsKey(track.Id);

		public int Count => this.traks.Count;

		public Track GetTrack(string title, string albumName)
		{
			if (!this.albums.ContainsKey(albumName) || !this.albums[albumName].ContainsKey(title))
			{
				throw new ArgumentException();
			}

			return this.albums[albumName][title];
		}

		public IEnumerable<Track> GetAlbum(string albumName)
		{
			if (!this.albums.ContainsKey(albumName))
			{
				throw new ArgumentException();
			}

			return this.albums[albumName].Values.OrderByDescending(t => t.Plays);
		}

		public void AddToQueue(string trackName, string albumName)
		{
			if (!this.albums.ContainsKey(albumName) || !this.albums[albumName].ContainsKey(trackName))
			{
				throw new ArgumentException();
			}

			this.listeningQueue.Enqueue(this.albums[albumName][trackName]);
		}

		public Track Play()
		{
			if (this.listeningQueue.Count == 0)
			{
				throw new ArgumentException();
			}

			Track track = this.listeningQueue.Dequeue();
			track.Plays++;

			return track;
		}

		public void RemoveTrack(string trackTitle, string albumName)
		{
			if (!this.albums.ContainsKey(albumName) || !this.albums[albumName].ContainsKey(trackTitle))
			{
				throw new ArgumentException();
			}

			Track track = this.albums[albumName][trackTitle];

			this.albums[albumName].Remove(track.Title);
			this.traks.Remove(track);
			this.traksById.Remove(track.Id);
			this.listeningQueue = new Queue<Track>(this.listeningQueue.Where(t => t.Id != track.Id));
		}

		public IEnumerable<Track> GetTracksInDurationRangeOrderedByDurationThenByPlaysDescending(int lowerBound, int upperBound) => this.traks.Where(t => t.DurationInSeconds >= lowerBound && t.DurationInSeconds <= upperBound)
				.OrderBy(t => t.DurationInSeconds)
				.ThenByDescending(t => t.Plays);







		public IEnumerable<Track> GetTracksOrderedByAlbumNameThenByPlaysDescendingThenByDurationDescending() => this.traks
			.OrderBy(t => t.Album)
			.ThenByDescending(t => t.Plays)
			.ThenByDescending(t => t.DurationInSeconds);


		public Dictionary<string, List<Track>> GetDiscography(string artistName)
		{
			IEnumerable<Track> songs = this.traks.Where(t => t.Artist == artistName);

			if (songs.Count() == 0)
			{
				throw new ArgumentException();
			}

			return songs.GroupBy(t => t.Album).ToDictionary(t => t.Key, t => t.ToList());
		}


	}
}
